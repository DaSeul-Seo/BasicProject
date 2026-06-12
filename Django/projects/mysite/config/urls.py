"""
URL configuration for config project.

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/6.0/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
# 페이지 요청이 발생하면 가장 먼저 호출되는 파일
# URL과 뷰 함수를 연결하는 역할
from django.contrib import admin
from django.urls import path, include

# from pybo import views -> URL매핑 추가할 때마다 수정해야하기에 include사용

urlpatterns = [
    path('admin/', admin.site.urls),
    path('pybo/', include('pybo.urls')), # pybo/ URL이 요청되면 pybo.urls 호출
]
