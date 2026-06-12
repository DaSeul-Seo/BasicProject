import datetime

from pydantic import BaseModel, field_validator
from domain.user.user_schema import User # 글쓴이 정보
from domain.comment.comment_schema import Comment

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

class AnswerCreate(BaseModel):
    content: str

    # content 값이 저장될 때 실행
    @field_validator('content')
    def not_empty(cls, v):
        if not v or not v.strip():
            raise ValueError('빈 값은 허용되지 않습니다.')
        return v

# 답변 조회
class Answer(BaseModel):
    id: int
    content: str
    create_date: datetime.datetime
    user: User | None
    question_id: int
    modify_date: datetime.datetime | None = None
    voter: list[User] = []
    answer_comments: list[Comment] = []

# 답변 수정
class AnswerUpdate(AnswerCreate):
    answer_id: int

# 답변 삭제
class AnswerDelete(BaseModel):
    answer_id: int

# 답변 추천
class AnswerVote(BaseModel):
    answer_id: int

# 답변 목록
class AnsertList(BaseModel):
    total:int = 0
    answer_list: list[Answer] = []