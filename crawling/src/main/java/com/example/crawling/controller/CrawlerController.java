package com.example.crawling.controller;

import java.util.List;

// REST API 관련 import
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;
import lombok.RequiredArgsConstructor;

import com.example.crawling.dto.GoogleResult;
import com.example.crawling.dto.NaverResult;
import com.example.crawling.dto.SerpResult;
import com.example.crawling.dto.YoutubeResult;
import com.example.crawling.service.GoogleCrawlerService;
import com.example.crawling.service.SeleniumService;
import com.example.crawling.service.SerpApiService;

@RestController
@RequiredArgsConstructor
public class CrawlerController {
    private final SeleniumService seleniumService;
    private final GoogleCrawlerService googleCrawlerService;
    private final SerpApiService serpApiService;

    @GetMapping("/google")
    public List<SerpResult> googleSearch(@RequestParam("keyword") String keyword) {
        // Service 실행
        return serpApiService.googleSearch(keyword);
    }

    @GetMapping("/naver")
    public List<SerpResult> naverSearch(@RequestParam("keyword") String keyword) {
        // Service 실행
        return serpApiService.naverSearch(keyword);
    }

    @GetMapping("/youtube")
    public List<SerpResult> youtubeSearch(@RequestParam("keyword") String keyword) {
        // Service 실행
        return serpApiService.youtubeSearch(keyword);
    }
}
