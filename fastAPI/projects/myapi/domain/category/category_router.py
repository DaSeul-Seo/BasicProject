from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session
from database import get_db
from models import Category

router = APIRouter(
    prefix="/api/category"
)

@router.get("/list")
def category_list(db: Session = Depends(get_db)):
    return db.query(Category) \
        .filter(Category.del_yn == 'N') \
        .all()