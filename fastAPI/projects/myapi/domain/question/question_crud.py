from datetime import datetime

from domain.question.question_schema import QuestionCreate, QuestionUpdate
from models import Question, User, Answer, Category
from sqlalchemy.orm import Session
from sqlalchemy import func, and_

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

# def get_question_list(db: Session):
#     question_list = db.query(Question)\
#         .order_by(Question.create_date.desc())\
#         .all()
#     return question_list

# 질문 리스트 가져오기
# def get_question_list(db: Session, skip: int = 0, limit: int = 10):
#     _question_list = db.query(Question)\
#         .filter(Question.del_yn == 'N')\
#         .order_by(Question.create_date.desc())
    
#     total = _question_list.count()
#     question_list = _question_list.offset(skip).limit(limit).all()

#     return total, question_list # (전체 건수, 페이징 적용된 질문 목록)

# 질문 리스트 가져오기(+검색)
def get_question_list(db: Session,
                      skip: int = 0,
                      limit: int = 10,
                      keyword: str = '',
                      category_id: int = 0):
    # question_list = db.query(Question)
    question_list = db.query(
        Question,
        func.count(Answer.id).label("answer_count")
    ).outerjoin(
        Answer,
        and_(
            Answer.question_id == Question.id,
            Answer.del_yn == 'N'
        )
    ).filter(
        Question.del_yn == 'N'
    )

    # 카페고리 필터 추가
    if category_id != 0:
        question_list = question_list.filter(
            Question.category_id == category_id,
            Question.del_yn == 'N'
        )

    # 검색어가 있을 경우
    if keyword:
        search = '%%{}%%'.format(keyword)
        sub_query = db.query(Answer.question_id, Answer.content, User.username) \
            .outerjoin(User, and_(Answer.user_id == User.id)) \
            .filter(Answer.del_yn == 'N').subquery()
        
        question_list = question_list \
            .outerjoin(User) \
            .outerjoin(sub_query, and_(sub_query.c.question_id == Question.id)) \
            .filter(
                Question.del_yn == 'N',
                Question.subject.ilike(search) |        # 질문제목
                Question.content.ilike(search) |        # 질문내용
                User.username.ilike(search) |           # 질문 작성자
                sub_query.c.content.ilike(search) |     # 답변내용
                sub_query.c.username.ilike(search)      # 답변작성자
            )
    
    #total = question_list.distinct().count()
    total = question_list.group_by(Question.id).count()
    question_list = question_list \
        .group_by(Question.id) \
        .order_by(Question.create_date.desc()) \
        .offset(skip).limit(limit).distinct().all()

    return total, question_list # (전체 건수, 페이징 적용된 질문 목록)



# 질문 가져오기
def get_question(db: Session, question_id: int):
    # question = db.query(Question).get(question_id)
    question = db.query(Question)\
        .filter(
            Question.id == question_id,
            Question.del_yn == 'N'
        )\
        .first()
    return question

# 질문 등록
def create_question(db: Session, question_create: QuestionCreate, user: User):
    db_question = Question(subject = question_create.subject,
                           content = question_create.content,
                           category_id = question_create.category_id,
                           create_date = datetime.now(),
                           user = user)
    db.add(db_question)
    db.commit()

# 질문 수정
def update_question(db: Session,
                    db_question: Question,
                    question_update: QuestionUpdate):
    db_question.subject = question_update.subject
    db_question.content = question_update.content
    db_question.category_id = question_update.category_id
    db_question.modify_date = datetime.now()
    db.add(db_question)
    db.commit()

# 질문 삭제
def delete_question(db: Session,
                    db_question: Question):
    
    # DB 수정 로직이라 crud에서 처리
    db_question.del_yn = 'Y'
    db_question.delete_date = datetime.now()
    db.add(db_question)
    db.commit()

# 질문 추천
def vote_question(db: Session,
                  db_question: Question,
                  db_user: User):
    db_question.voter.append(db_user)
    db.commit()