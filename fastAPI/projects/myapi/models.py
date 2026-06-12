from sqlalchemy import Column, Integer, String, Text, DateTime, ForeignKey, Table
from sqlalchemy.orm import relationship

from database import Base

# ManyToMany
# 추천 = 질문과 답변에 추천인 속성 추가
# 하나의 질문에 여러명이 추천할 수 있고, 한 명이 여러개의 질문에 추천할 수 있다.
question_voter = Table(
    'question_voter',   # 테이블명
    Base.metadata,
    Column('user_id', Integer, ForeignKey('user.id'), primary_key=True),
    Column('question_id', Integer, ForeignKey('question.id'), primary_key=True)
)

answer_voter = Table(
    'answer_voter',
    Base.metadata,
    Column('user_id', Integer, ForeignKey('user.id'), primary_key=True),
    Column('question_id', Integer, ForeignKey('answer.id'), primary_key=True)
)

class Question(Base):
    __tablename__ = "question"

    id = Column(Integer, primary_key=True)
    subject = Column(String, nullable=False)
    content = Column(Text, nullable=False)
    create_date = Column(DateTime, nullable=False)
    # User모델을 Question 모델과 연결하기 위한 속성
    user_id = Column(Integer, ForeignKey("user.id"), nullable=True)
    # Question 모델에서 User 모델을 참조하기 위한 속성
    user = relationship("User", backref="question_users")
    # 수정일시
    modify_date = Column(DateTime, nullable=True)
    # 삭제여부
    del_yn = Column(String(1), server_default='N', nullable=False)
    # 삭제 시간
    delete_date = Column(DateTime, nullable=True)
    # 추천인
    # secondary : 실제 데이터는 question_voter 테이블에 저장되고 저장된 추천일 정보는 Question 모델의 voter속성을 통해 참조
    # backref : relationship속성 생성시, backref 중복되면 안됨
    voter = relationship('User', secondary=question_voter, backref='question_voters')
    # 카테고리
    category_id = Column(Integer, ForeignKey("category.id"), nullable = True)
    category = relationship("Category", backref="questions")

class Answer(Base):
    __tablename__ = "answer"

    id = Column(Integer, primary_key=True)
    content = Column(Text, nullable=False)
    create_date = Column(DateTime, nullable=False)
    # 답변을 질문과 연결하기 위해 : question 테이블의 id컬럼
    question_id = Column(Integer, ForeignKey("question.id"))
    # 답변 모델에서 질문 모델을 참조하기 위해 (ex. answer.question.subject)
    # 파라미터 : 참조할 모델명, 역참조 설정(답변을 거꾸로 참조하는 것)
    # => 한 질문에 여러 개의 답변이 달릴 수 있는데 역참조는 이 질문에 달린 답변들을 참고할 수 있게 함
    question = relationship("Question", backref="answers")
    # User모델을 Answer 모델과 연결하기 위한 속성
    user_id = Column(Integer, ForeignKey("user.id"), nullable=True)
    # Answer 모델에서 User 모델을 참조하기 위한 속성
    user = relationship("User", backref="answer_users")
    # 수정일시
    modify_date = Column(DateTime, nullable=True)
    # 삭제여부
    del_yn = Column(String(1), server_default='N', nullable=False)
    # 삭제 시간
    delete_date = Column(DateTime, nullable=True)
    # 추천인
    # secondary : 실제 데이터는 question_voter 테이블에 저장되고 저장된 추천일 정보는 Answer 모델의 voter속성을 통해 참조
    # backref : relationship속성 생성시, backref 중복되면 안됨
    voter = relationship('User', secondary=answer_voter, backref='answer_voters')
    # 추천순 정렬
    voter_count = Column(Integer, default=0)

class User(Base):
    __tablename__ = "user"

    # username과 email은 중복저장되지 않는다 (unique)
    id = Column(Integer, primary_key=True)
    username = Column(String, unique=True, nullable=False)
    password = Column(String, nullable=False)
    email = Column(String, unique=True, nullable=False)
    # 삭제여부
    del_yn = Column(String(1), server_default='N', nullable=False)
    # 삭제 시간
    delete_date = Column(DateTime, nullable=True)

# 댓글
class Comment(Base):
    __tablename__ = "comment"

    # 질문, 답변 둘 다 댓글 달 수 있도록
    id = Column(Integer, primary_key=True)
    content = Column(Text, nullable=False)
    create_date = Column(DateTime, nullable=False)
    question_id = Column(Integer, ForeignKey("question.id"))
    question = relationship("Question", backref="question_comments")
    answer_id = Column(Integer, ForeignKey("answer.id"))
    answer = relationship("Answer", backref="answer_comments")
    user_id = Column(Integer, ForeignKey("user.id"))
    user = relationship("User", backref="user_comments")
    modify_date = Column(DateTime, nullable=True)
    del_yn = Column(String(1), server_default='N', nullable=False)
    delete_date = Column(DateTime, nullable=True)

class Category(Base):
    __tablename__ = "category"

    id = Column(Integer, primary_key=True)
    subject = Column(String, nullable=False)
    del_yn = Column(String(1), server_default='N', nullable=False)
    delete_date = Column(DateTime, nullable=True)
    