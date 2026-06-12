from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from starlette import status

# from database import SessionLocal
from database import get_db
from domain.question import question_schema, question_crud
from domain.answer import answer_crud
from domain.comment import comment_crud
from domain.user.user_router import get_current_user
from models import User
# from models import Question => question_crud로 대체

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

# 라우팅 : FastAPI가 요청받은 URL을 해석하여 그에 맞는 함수를 실행하여 그 결과를 반환
# prefix : URL에 항상 포함되어야 하는 값
router = APIRouter(
    prefix="/api/question"
)

# /api/question/list
# 페이지네이션 포함된 리스트
@router.get("/list", response_model=question_schema.QuestionList)
def question_list(db: Session = Depends(get_db),
                  page: int = 0, size: int = 10, keyword: str = '',
                  category_id: int = 0):
    total, rows = question_crud.get_question_list(
        db, 
        skip=page*size, 
        limit=size, 
        keyword=keyword,
        category_id=category_id)

    _question_list = []
    
    for question, answer_count in rows:
        question.answer_count = answer_count
        _question_list.append(question)

    return {
        'total': total,
        'question_list': _question_list
    }

# 리턴값 : Question 스키마
# @router.get("/list", response_model=list[question_schema.Question])
# # 방법3
# # Depends는 매개변수로 전달받은 함수를 호출하여 그 결과를 리턴
# # db 객체에 get_db 제너레이터 함수가 yield를 통해 생성한 세션 객체가 주입됨
# def question_list(db: Session = Depends(get_db)):
#     # _question_list = db.query(Question).order_by(Question.create_date.desc()).all()
#     _question_list = question_crud.get_question_list(db)
#     return _question_list

# 방법2
# def question_list():
#     with get_db() as db:
#         _question_list = db.query(Question).order_by(Question.create_date.desc()).all()
#     return _question_list

# 방법1
# def question_list():
#     # db 세션 생성
#     db = SessionLocal()
#     # 질문 목록 조회
#     _question_list = db.query(Question).order_by(Question.create_date.desc()).all()
#     # 사용한 세션을 커넥션 풀에 반환(세션 종료 X)
#     # db 세션 객체 생성 후 db.close()를 수행하지 않으면 SQLAlchemy가 사용하는 커넥션 풀에 db세선이 반환되지 않아 문제 발생
#     db.close()
#     return _question_list

# 가변적인 숫자값
@router.get("/detail/{question_id}", response_model = question_schema.Question)
def question_detail(question_id: int, db: Session = Depends(get_db)):
    question = question_crud.get_question(db, question_id = question_id)
    _, question_comment_list = comment_crud.get_question_comment_list(db, question_id = question_id)
    # _, answer_list = answer_crud.get_answer_list(db, question_id = question_id)
    
    # # 답변마다 댓글
    # for answer in answer_list:
    #     _, answer_comment_list = comment_crud.get_answer_comment_list(db, answer.id)
    
    #     answer.answer_comments = answer_comment_list

    # question.answers = answer_list
    question.question_comments = question_comment_list
    
    return question

# 질문 등록
@router.post("/create", status_code = status.HTTP_204_NO_CONTENT)
def question_create(_question_create: question_schema.QuestionCreate,
                    db: Session = Depends(get_db),
                    current_user: User = Depends(get_current_user)):
    question_crud.create_question(db = db,
                                  question_create = _question_create,
                                  user = current_user)

# 질문 수정
@router.put("/update", status_code = status.HTTP_204_NO_CONTENT)
def question_update(_question_update: question_schema.QuestionUpdate,
                    db: Session = Depends(get_db),
                    current_user: User = Depends(get_current_user)):
    # 질문 정보 가져오기
    db_question = question_crud.get_question(db, question_id=_question_update.question_id)
    
    # 질문 정보가 없는 경우
    if not db_question:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "데이터를 찾을 수 없습니다.")
    
    # 작성한 사용자만 수정할 수 있다.
    if current_user.id != db_question.user.id:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "수정 권한이 없습니다.")

    question_crud.update_question(db = db,
                                  db_question = db_question,
                                  question_update = _question_update)

# 질문 삭제
@router.delete("/delete", status_code = status.HTTP_204_NO_CONTENT)
def question_delete(_question_delete: question_schema.QuestionDelete,
                    db: Session = Depends(get_db),
                    current_user = Depends(get_current_user)):
    # 질문 정보 가져오기
    db_question = question_crud.get_question(db, question_id=_question_delete.question_id)

    # 질문 정보가 없는 경우
    if not db_question:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "데이터를 찾을 수 없습니다.")
    
    # 작성한 사용자만 삭제할 수 있다.
    if current_user.id != db_question.user.id:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "삭제 권한이 없습니다.")
    
    question_crud.delete_question(db = db,
                                  db_question = db_question)

# 질문 추천
@router.post("/vote", status_code = status.HTTP_204_NO_CONTENT)
def question_vote(_question_vote: question_schema.QuestionVote,
                  db: Session = Depends(get_db),
                  current_user: User = Depends(get_current_user)):
    db_question = question_crud.get_question(db, question_id = _question_vote.question_id)

    if not db_question:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "데이터를 찾을 수 없습니다.")
    
    question_crud.vote_question(db, db_question = db_question, db_user = current_user)