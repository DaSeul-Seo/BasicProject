from datetime import datetime
from sqlalchemy.orm import Session
from domain.comment.comment_schema import CommentCreate, CommentUpdate
from models import Question, Answer, User, Comment

# 질문 댓글 등록
def create_comment_question(db: Session,
                            question: Question,
                            comment_create:CommentCreate,
                            user: User):
    db_comment = Comment(question = question,
                         content = comment_create.content,
                         create_date = datetime.now(),
                         user = user)
    
    db.add(db_comment)
    db.commit()

# 답변 댓글 등록
def create_comment_answer(db: Session,
                          answer: Answer,
                          comment_create: CommentCreate,
                          user: User):
    db_comment = Comment(answer = answer,
                         content = comment_create.content,
                         create_date = datetime.now(),
                         user = user)
    
    db.add(db_comment)
    db.commit()

# 댓글 가져오기
def get_comment(db: Session,
                comment_id: int):
    comment = db.query(Comment) \
        .filter(
            Comment.id == comment_id,
            Comment.del_yn == 'N'
        ) \
        .first()
    
    return comment

# 질문 댓글 리스트 가져오기
def get_question_comment_list(db: Session,
                              question_id: int):
    comment_list = db.query(Comment) \
        .filter(
            Comment.question_id == question_id,
            Comment.del_yn == 'N'
        ) \
        .order_by(Comment.create_date.asc()) \
        .all()
    
    total = len(comment_list)
    
    return total, comment_list

# 답변 댓글 리스트 가져오기
def get_answer_comment_list(db: Session,
                            answer_id: int):
    comment_list = db.query(Comment) \
        .filter(
            Comment.answer_id == answer_id,
            Comment.del_yn == 'N'
        ) \
        .order_by(Comment.create_date.asc()) \
        .all()
    
    total = len(comment_list)

    return total, comment_list

# 댓글 수정
def update_comment(db: Session,
                   db_comment: Comment,
                   comment_update: CommentUpdate):
    db_comment.content = comment_update.content
    db_comment.modify_date = datetime.now()
    db.add(db_comment)
    db.commit()

# 댓글 삭제
def delete_comment(db: Session,
                   db_comment: Comment):
    db_comment.del_yn = 'Y'
    db_comment.delete_date = datetime.now()
    db.add(db_comment)
    db.commit()

