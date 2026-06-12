package com.example.crawling.service;

// Jsoup
import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;
import org.openqa.selenium.json.JsonOutput;
// Spring
import org.springframework.stereotype.Service;

import com.example.crawling.dto.GoogleResult;

// URL 인코딩
import java.net.URLEncoder;

// UTF-8 인코딩
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;

@Service
public class GoogleCrawlerService {
    private final String googleSearchUrl = "https://www.google.com/search?q=";

    // 구글 검색
    public List<GoogleResult> googleSearch(String keyword) {
        // 결과 저장 리스트
        List<GoogleResult> resultList = new ArrayList<>();

        try {
            // 검색어 URL 인코딩 (한글 깨짐 방지)
            String encodedKeyword = URLEncoder.encode(keyword, StandardCharsets.UTF_8);

            // 구글 검색 URL
            String url = googleSearchUrl + encodedKeyword;

            // 구글 HTML 가져오기
            Document doc = Jsoup.connect(url)
                                .userAgent("Mozilla/5.0") // 브라우저처럼 보이게 User-Agent 설정
                                .timeout(5000)
                                .get(); // html 가져오기
                                
            System.out.println(doc.html());
            
            // 검색 결과 영역 가져오기
            Elements results = doc.select("div.g");

            // 검색 결과 반복
            for (Element result : results) {
                // 제목 태그 가져오기
                Element titleElement = result.selectFirst("h3");

                // 링크 태그 가져오기
                Element linkElement = result.selectFirst("a");

                // 제목, 링크 둘다 있을때만
                if (titleElement != null && linkElement != null) {
                    // 제목&링크 추출
                    String title = titleElement.text();
                    String link = linkElement.attr("href");

                    // dto 저장
                    resultList.add(new GoogleResult(
                        title,
                        link,
                        ""
                    ));
                }
            }
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        return resultList;
    }
}
