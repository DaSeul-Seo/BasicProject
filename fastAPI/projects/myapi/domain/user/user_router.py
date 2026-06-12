from datetime import timedelta, datetime

from fastapi import APIRouter, HTTPException
from fastapi import Depends
from fastapi.security import OAuth2PasswordRequestForm, OAuth2PasswordBearer
from jose import jwt, JWTError
from sqlalchemy.orm import Session
from starlette import status

from database import get_db
from domain.user import user_crud, user_schema
from domain.user.user_crud import pwd_context

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

# 토큰의 유효기간 (분단위)
ACCESS_TOKEN_EXPIRE_MINUTES = 60 * 24
# 암호화시 사용하는 64자기 랜덤 문자열
SECRET_KEY = "4b63eb5917c83979efa158e9aa1372428044135e7b2bb737972880bc861edcda"
# 토큰 생성시 사용하는 알고리즘
ALGORITHM = "HS256"
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/api/user/login")

router = APIRouter(
    prefix="/api/user",
)

@router.post("/create", status_code=status.HTTP_204_NO_CONTENT)
def user_create(_user_create: user_schema.UserCreate, db: Session = Depends(get_db)):
    # 중복값에 대한 예외 처리
    user = user_crud.get_existing_user(db, user_create=_user_create)
    if user:
        raise HTTPException(status_code=status.HTTP_409_CONFLICT,
                            detail="이미 존재하는 사용자입니다.")
    
    user_crud.create_user(db = db, user_create = _user_create)

@router.post("/login", response_model=user_schema.Token)
def login_for_access_token(form_data: OAuth2PasswordRequestForm = Depends(),
                           db: Session = Depends(get_db)):
    # 사용자 체크
    user = user_crud.get_user(db, form_data.username)
    # pwd_context.verify : 암호화되지 않은 비밀번호를 암호화하여 데이터베이스에 저장된 암호화 일지하는지 판단
    if not user or not pwd_context.verify(form_data.password, user.password):
        raise HTTPException(
            status_code = status.HTTP_401_UNAUTHORIZED,
            detail = "Incorrect username or password",
            headers = {"WWW-Authenticate": "Bearer"}, # 인증 방식의 추가정보
        )

    # 토큰 발급
    data = {
        "sub": user.username,
        "exp": datetime.utcnow() + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    }
    access_token = jwt.encode(data, SECRET_KEY, algorithm=ALGORITHM)

    return {
        "access_token": access_token,
        "token_type": "bearer",
        "username": user.username
    }

# 글쓴이 정보 엊어오기
# 1. 프론트엔드에서 로그인을 성공한 후에 액세스 토큰을 저장
# 2. 백엔드 API 호출시 헤더 정보에 액세스 토큰을 포함하여 요청
# 3. 백엔드에서 액세스 토큰을 분석하여 사용자명 취득
# 4. 사용자명으로 사용자 조회

# 헤더 정보의 토큰값으로 사용자 정보를 조회
def get_current_user(token: str = Depends(oauth2_scheme),
                     db: Session = Depends(get_db)):
    credentials_exception = HTTPException(
        status_code = status.HTTP_401_UNAUTHORIZED,
        detail = "Could not validate credentials",
        headers = { "WWW-Authenticate": "Bearer" },
    )

    try:
        # jwt.decode : 토큰은 복호화하여 토큰에 담겨 있는 사용자명을 얻을 수 있다.
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        username: str = payload.get("sub")
        if username is None:
            raise credentials_exception
    except JWTError:
        raise credentials_exception
    else:
        user = user_crud.get_user(db, username=username)
        if user is None:
            raise credentials_exception
        return user
