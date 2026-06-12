from datetime import datetime

from sqlalchemy.orm import Session

from domain.answer.answer_schema import AnswerCreate, AnswerUpdate
from models import Question, Answer, User

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

def create_answer(db: Session,
                  question: Question,
                  answer_create: AnswerCreate,
                  user: User): # 글쓴이 저장
    db_answer = Answer(
        question = question,
        content = answer_create.content,
        create_date = datetime.now(),
        user = user
    )

    db.add(db_answer)
    db.commit()

# 답변 리스트 가져오기(페이징)
def get_answer_list(db: Session,
                    question_id: int,
                    skip: int = 0,
                    limit = 5,
                    sort_by: str = 'create_date',
                    desc: bool = True):
    # 정렬 속성
    sort_column = getattr(Answer, sort_by)
    if desc:
        sort_column = sort_column.desc()
    else:
        sort_column = sort_column.asc()

    _answer_list = db.query(Answer) \
        .filter(
            Answer.question_id == question_id,
            Answer.del_yn == 'N'
        ) \
        .order_by(sort_column)
    
    total = _answer_list.count()
    answer_list = _answer_list.offset(skip).limit(limit).all()

    return total, answer_list # 전체 건수, 페이징 적용된 답변 목록

# 답변 리스트 가져오기
# def get_answer_list(db: Session,
#                     question_id: int):
#     answer_list = db.query(Answer)\
#         .filter(
#             Answer.question_id == question_id,
#             Answer.del_yn == 'N'
#         )\
#         .order_by(Answer.create_date.asc())\
#         .all()
    
#     return answer_list

# 답변 가져오기
def get_answer(db: Session,
               answer_id: int):
    return db.query(Answer).get(answer_id)

# 답변 수정하기
def update_answer(db: Session,
                  db_answer: Answer,
                  answer_update: AnswerUpdate):
    db_answer.content = answer_update.content
    db_answer.modify_date = datetime.now()
    db.add(db_answer)
    db.commit()

# 답변 삭제하기
def delete_answer(db: Session,
                  db_answer: Answer):
    db_answer.del_yn = 'Y'
    db_answer.delete_date = datetime.now()
    db.add(db_answer)
    db.commit()

# 답변 추천
def vote_answer(db: Session,
                db_answer: Answer,
                db_user: User):
    db_answer.voter.append(db_user)
    db_answer.voter_count = db_answer.voter_count + 1
    db.commit()