# from contextlib

from sqlalchemy import create_engine, MetaData
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

# sqlite3 데이터베이스의 파일을 의미
# 프로젝트 루트 디렉처리에 저장
SQLALCHEMY_DATABASE_URL = "sqlite:///./myapi.db"

# 데이터베이스에 접속하기 위해 필요한 클래스
# create_engine : 커넥션 풀을 생성
# 커넥션 풀 : 데이터베이스에 접속하는 객체를 일정 객수만큼 만들어 놓고 돌려가며 사용하는 것
#          : 데이터베이스에 접속하는 세션수를 제어하고, 또 세션 접속에 소요되는 시간을 줄이고자 하는 용도로 사용
engine = create_engine(
    SQLALCHEMY_DATABASE_URL, connect_args={ "check_same_thread": False }
)

SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# declarative_base 함수에 의해 반환된 Base 클래스는 데이터베이스 모델을 구성할 때 사용되는 클래스
Base = declarative_base()

# MetaData를 사용해서 새로 정의
naming_convention = {
    "ix": 'ix_%(column_0_label)s',
    "uq": "uq_%(table_name)s_%(column_0_name)s",
    "ck": "ck_%(table_name)s_%(column_0_name)s",
    "fk": "fk_%(table_name)s_%(column_0_name)s_%(referred_table_name)s",
    "pk": "pk_%(table_name)s"
}
Base.metadata = MetaData(naming_convention=naming_convention)


# @contextlib.contextmanager : main에서 제너레이터 함수를 썼기에 제거
def get_db():
    db = SessionLocal()
    
    try:
        yield db
    finally:
        db.close()