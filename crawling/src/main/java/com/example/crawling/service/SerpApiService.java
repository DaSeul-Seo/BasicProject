package com.example.crawling.service;

import com.example.crawling.dto.GoogleResult;
import com.example.crawling.dto.NaverResult;
import com.example.crawling.dto.SerpResult;
import com.example.crawling.dto.YoutubeResult;
// Json 파싱
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;

// Spring yml 파일 읽기
import org.springframework.beans.factory.annotation.Value;
// Spring Service
import org.springframework.stereotype.Service;

// URL 인코딩
import java.net.URLEncoder;

// utf-8
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
// Java HTTP Client
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

@Service
public class SerpApiService {
    // application.yml 값 가져오기
    @Value("${serpapi.api-key}")
    private String apiKey;

    @Value("${serpapi.url}")
    private String serpUrl;

    // 현재 profile 확인
    @Value("${spring.profiles.active}")
    private String activeProfile;

    public List<SerpResult> googleSearch(String keyword) {
        // 결과 저장 리스트
        List<SerpResult> resultList = new ArrayList<>();

        try {
            // 검색어 URL 인코딩
            String encodedKeyword = URLEncoder.encode(keyword, StandardCharsets.UTF_8);

            // 요청 URL
            String url = serpUrl
                        + "?engine=google"
                        + "&q=" + encodedKeyword
                        + "&api_key=" + apiKey;
            
            // HTTP Client 생성
            HttpClient client = HttpClient.newHttpClient();

            // 요청 객체 생성
            HttpRequest request = HttpRequest.newBuilder()
                                .uri(URI.create(url))   // 요청 주소 설정
                                .GET()  // GET 요청
                                .build();
            
            // 요청
            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());

            System.out.println(response.body());

            // Json 파서
            ObjectMapper mapper = new ObjectMapper();

            // Json 읽기
            JsonNode root = mapper.readTree(response.body());

            // 검색 결과 배열
            JsonNode organicResults = root.get("organic_results");

            // 결과 반복
            for (JsonNode item : organicResults) {
                SerpResult dto = new SerpResult();

                // 결과 저장
                dto.setTitle(item.get("title").asText());
                dto.setLink(item.get("link").asText());
                dto.setSnippet(item.get("snippet").asText());

                resultList.add(dto);
            }
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        return resultList;
    }

    public List<SerpResult> naverSearch(String keyword) {
        // 결과 저장 리스트
        List<SerpResult> resultList = new ArrayList<>();

        try {
            // 검색어 URL 인코딩
            String encodedKeyword = URLEncoder.encode(keyword, StandardCharsets.UTF_8);

            // 요청 URL
            String url = serpUrl
                        + "?engine=naver"
                        + "&query=" + encodedKeyword
                        + "&api_key=" + apiKey;
            
            // HTTP Client 생성
            HttpClient client = HttpClient.newHttpClient();

            // 요청 객체 생성
            HttpRequest request = HttpRequest.newBuilder()
                                .uri(URI.create(url))   // 요청 주소 설정
                                .GET()  // GET 요청
                                .build();
            
            // 요청
            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());

            // Json 파서
            ObjectMapper mapper = new ObjectMapper();

            // Json 읽기
            JsonNode root = mapper.readTree(response.body());

            // 검색 결과 배열
            JsonNode viewResults = root.get("view_results");

            // 결과 반복
            for (JsonNode item : viewResults) {
                SerpResult dto = new SerpResult();

                // 결과 저장
                dto.setTitle(item.get("title").asText());
                dto.setLink(item.get("link").asText());
                dto.setSnippet(item.get("snippet").asText());

                resultList.add(dto);
            }
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        return resultList;
    }

    public List<SerpResult> youtubeSearch(String keyword) {
        // 결과 저장 리스트
        List<SerpResult> resultList = new ArrayList<>();

        try {
            // 검색어 URL 인코딩
            String encodedKeyword = URLEncoder.encode(keyword, StandardCharsets.UTF_8);

            // 요청 URL
            String url = serpUrl
                        + "?engine=youtube"
                        + "&search_query=" + encodedKeyword
                        + "&api_key=" + apiKey;
            
            // HTTP Client 생성
            HttpClient client = HttpClient.newHttpClient();

            // 요청 객체 생성
            HttpRequest request = HttpRequest.newBuilder()
                                .uri(URI.create(url))   // 요청 주소 설정
                                .GET()  // GET 요청
                                .build();
            
            // 요청
            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());

            System.out.println(response.body());

            // Json 파서
            ObjectMapper mapper = new ObjectMapper();

            // Json 읽기
            JsonNode root = mapper.readTree(response.body());

            // 검색 결과 배열
            JsonNode videoResults = root.get("video_results");

            // 결과 반복
            for (JsonNode item : videoResults) {
                SerpResult dto = new SerpResult();

                // 결과 저장
                dto.setTitle(item.get("title").asText());
                dto.setLink(item.get("link").asText());
                dto.setSnippet(item.get("description").asText());

                resultList.add(dto);
            }
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        return resultList;
    }

}
