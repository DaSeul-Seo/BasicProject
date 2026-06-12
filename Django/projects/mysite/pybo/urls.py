# config/urls.py에서 매핑
from django.urls import path

from . import views

urlpatterns = [
    path('', views.index),
]