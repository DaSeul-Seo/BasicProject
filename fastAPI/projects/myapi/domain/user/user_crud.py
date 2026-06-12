from passlib.context import CryptContext
from sqlalchemy.orm import Session
from domain.user.user_schema import UserCreate
from models import User

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

# 로그인 시 사용자로부터 입력받은 비밀번호를 동일한 방식으로 암호화한 후,
# 데이터베이스에 저장된 값과 비교하여 비밀번호가 동일한지 체크할 수 있다.
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

def create_user(db: Session, user_create: UserCreate):
    db_user = User(username = user_create.username,
                   password = pwd_context.hash(user_create.password1),
                   email = user_create.email)
    
    db.add(db_user)
    db.commit()

# 중복값에 대한 예외처리
def get_existing_user(db: Session, user_create: UserCreate):
    return db.query(User).filter(
        (User.username == user_create.username) | 
        (User.email == user_create.email)
    ).first()


# 유저정보 가져오기 : username으로 User 데이터를 가져와 비밀번호 비교 해야하기에
def get_user(db: Session, username: str):
    return db.query(User).filter(User.username == username).first()