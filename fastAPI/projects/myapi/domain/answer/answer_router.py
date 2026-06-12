from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from starlette import status

from database import get_db
from domain.answer import answer_schema, answer_crud
from domain.question import question_crud
from domain.comment import comment_crud
from domain.user.user_router import get_current_user
from models import User

# crud에서 데이터를 추출 -> schema에서 받기(response_model) -> router에서 함수 실행 -> 화면에 뿌리기

router = APIRouter(
    prefix="/api/answer"
)


@router.post("/create/{question_id}", status_code = status.HTTP_204_NO_CONTENT)
def answer_create(question_id: int,
                  _answer_create: answer_schema.AnswerCreate,
                  db: Session = Depends(get_db),
                  current_user: User = Depends(get_current_user)):
    
    question = question_crud.get_question(db, question_id=question_id)

    if not question:
        # 해당 질문이 존재하지 않을 경우
        raise HTTPException(status_code = 404, detail = "Question Not Found")
    answer_crud.create_answer(db, question=question,
                              answer_create=_answer_create,
                              user = current_user)
    
# 답변 조회
@router.get("/detail/{answer_id}", response_model = answer_schema.Answer)
def answer_detail(answer_id: int,
                  db: Session = Depends(get_db)):
    answer = answer_crud.get_answer(db, answer_id = answer_id)
    return answer

# 답변 수정
@router.put("/update", status_code=status.HTTP_204_NO_CONTENT)
def answer_update(_answer_update: answer_schema.AnswerUpdate,
                  db: Session = Depends(get_db),
                  current_user: User = Depends(get_current_user)):
    # 답변 가져오기
    db_answer = answer_crud.get_answer(db, answer_id = _answer_update.answer_id)

    # 답변 정보가 없는 경우
    if not db_answer:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST,
                            detail="데이터를 찾을수 없습니다.")
    
    # 작성한 사용자만 수정할 수 있다.
    if current_user.id != db_answer.user.id:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST,
                            detail="수정 권한이 없습니다.")
    
    answer_crud.update_answer(db = db,
                              db_answer = db_answer,
                              answer_update = _answer_update)

# 답변 삭제
@router.delete("/delete", status_code = status.HTTP_204_NO_CONTENT)
def question_delete(_answer_delete: answer_schema.AnswerDelete,
                    db: Session = Depends(get_db),
                    current_user = Depends(get_current_user)):
    # 질문 정보 가져오기
    db_answer = answer_crud.get_answer(db, answer_id=_answer_delete.answer_id)

    # 질문 정보가 없는 경우
    if not db_answer:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "데이터를 찾을수 없습니다.")
    
    # 작성한 사용자만 삭제할 수 있다.
    if current_user.id != db_answer.user.id:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "삭제 권한이 없습니다.")
    
    answer_crud.delete_answer(db = db,
                              db_answer = db_answer)

# 답변 추천
@router.post("/vote", status_code = status.HTTP_204_NO_CONTENT)
def answer_vote(_answer_vote: answer_schema.AnswerVote,
                db: Session = Depends(get_db),
                current_user: User = Depends(get_current_user)):
    db_answer = answer_crud.get_answer(db, answer_id = _answer_vote.answer_id)

    if not db_answer:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "데이터를 찾을 수 없습니다.")
    
    answer_crud.vote_answer(db, db_answer = db_answer, db_user = current_user)

# 답변 리스트 (페이지네이션 포함)
@router.get("/list", response_model=answer_schema.AnsertList)
def answer_list(question_id: int,
                db: Session = Depends(get_db),
                sort_by: str = 'create_date',
                desc: bool = True,
                page: int = 0, size: int = 5):
    question = question_crud.get_question(db, question_id=question_id)

    if not question:
        raise HTTPException(status_code = status.HTTP_400_BAD_REQUEST,
                            detail = "데이터를 찾을 수 없습니다.")
    
    total, _answer_list = answer_crud.get_answer_list(
        db, 
        question_id = question_id, 
        skip = page*size,
        limit = size,
        sort_by = sort_by,
        desc = desc)
        
    # 답변마다 댓글
    for answer in _answer_list:
        _, answer_comment_list = comment_crud.get_answer_comment_list(db, answer.id)
    
        answer.answer_comments = answer_comment_list

    return {
        'total': total,
        'answer_list': _answer_list
    }