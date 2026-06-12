package com.example.linkarchive.util;

public enum ErrorCode {
    
    USER_NOT_FOUND("사용자를 찾을 수 없습니다."),
    PASSWORD_NOT_MATCH("현재 비밀번호가 다릅니다."),
    PASSWORD_CONFIRM_NOT_MATCH("새 비밀번호가 일치하지 않습니다."),
    DUPLICATE_USER_ID("이미 존재하는 아이디입니다."),
    LINK_NOT_FOUND("링크를 찾을 수 없습니다."),
    URL_NOT_FOUND("URL 수집 실패"),
    GRANT_NOT_FOUND("권한이 없습니다.");

    private final String message;

    ErrorCode(String message) {
        this.message = message;
    }

    public String getMessage() {
        return message;
    }
}
