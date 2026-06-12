# 출력 스키마

import datetime

from pydantic import BaseModel, field_validator
from domain.answer.answer_schema import Answer
from domain.user.user_schema import User # 글쓴이 정보
from domain.comment.comment_schema import Comment
from domain.category.category_schema import Category

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

class Question(BaseModel):
    id: int
    subject: str
    content: str
    create_date: datetime.datetime
    answers: list[Answer] = [] # 답변 상세페이지용
    answer_count: int = 0 # 답변 갯수
    user: User | None
    modify_date: datetime.datetime | None = None
    voter: list[User] = []
    question_comments: list[Comment] = []
    category: Category | None = None

    class Config:
        from_attributes = True
    
class QuestionCreate(BaseModel):
    subject: str
    content: str
    category_id: int

    @field_validator('subject', 'content')
    def not_empty(cls, v):
        if not v or not v.strip():
            raise ValueError('빈 값은 허용되지 않습니다.')
        return v

class QuestionList(BaseModel):
    total: int = 0
    question_list: list[Question] = []

# 수정
class QuestionUpdate(QuestionCreate):
    # QuestionCreate 스키마에 이미 subject, content 항목이 있으므로
    # QuestionCreate 스키마를 상속하고 question_id 항목만 추가 (검증 메서드 동일하게 동작)
    question_id: int

# 삭제
class QuestionDelete(BaseModel):
    question_id: int

# 질문 추천
class QuestionVote(BaseModel):
    question_id: int