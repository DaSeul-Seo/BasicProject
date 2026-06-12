package com.example.crawling.dto;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class SerpResult {
    // 검색 결과 제목
    private String title;

    // 검색 결과 링크
    private String link;

    // 검색 결과 설명
    private String snippet;
}
