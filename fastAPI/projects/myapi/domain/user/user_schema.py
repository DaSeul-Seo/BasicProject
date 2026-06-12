from pydantic import BaseModel, field_validator, EmailStr
from pydantic_core.core_schema import FieldValidationInfo

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

class UserCreate(BaseModel):
    username: str
    password1: str
    password2: str
    email: EmailStr

    @field_validator('username', 'password1', 'password2', 'email')
    def not_empty(cls, v):
        if not v or not v.strip():
            raise ValueError('빈 값은 허용되지 않습니다.')
        return v

    @field_validator('password2')
    def passwords_match(cls, v, info: FieldValidationInfo):
        if 'password1' in info.data and v != info.data['password1']:
            raise ValueError('비밀번호가 일치하지 않습니다.')
        return v

# fastapi에 security 패키지 안에 OAuth2PasswordRequestForm 클래스 사용하기에 따로 만들 필요 없음
class Token(BaseModel):
    access_token: str
    token_type: str
    username: str

# 글쓴이 정보
class User(BaseModel):
    id: int
    username: str
    email: str