package com.example.crawling.service;

// Selenium WebDriver 자동 설치 라이브러리
import io.github.bonigarcia.wdm.WebDriverManager;

import java.util.List;

import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
// Selenium 브라우저 제어용
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
// 크롬 브라우저 드라이버
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;
// Spring Service 어노테이션
import org.springframework.stereotype.Service;

@Service
public class SeleniumService {
    public String googleSearch() {
        // 현재 설치된 chrome 버전에 맞는 chromedriver 자동 다운로드
        WebDriverManager.chromedriver().setup();

        // Chrome 옵션
        ChromeOptions options = new ChromeOptions();

        // Selenium 자도오하 표시 제거
        options.addArguments("--disable-blink-features=AutomationControlled");

        // 자동화 문구 제거
        options.setExperimentalOption(
                "excludeSwitches",
                new String[]{"enable-automation"}
        );

        // 자동화 확장 기능 비활성화
        options.setExperimentalOption(
                "useAutomationExtension",
                false
        );

        // 최대화 실행
        options.addArguments("--start-maximized");

        // chrome 브라우저 실행 (옵션 적용)
        WebDriver driver = new ChromeDriver(options);

        try {
            // 구글 페이지 접속
            driver.get("https://www.google.com");
    
            // 구글 창 검색
            WebElement searchBox = driver.findElement(By.name("q"));

            Thread.sleep(1000);

            // 검색어 입력
            searchBox.sendKeys("Srping Boot Selenium");

            Thread.sleep(1000);

            // 엔터 입력
            searchBox.sendKeys(Keys.ENTER);

            // 페이지 로딩 기다리기
            Thread.sleep(3000);

            // 검색 결과 제목 가져오기
            List<WebElement> results = driver.findElements(By.cssSelector("h3"));

            // 결과 저장
            StringBuilder sb = new StringBuilder();

            // 검색 결과 반복
            for (WebElement result : results) {
                // 제목 text 가져오기
                String title = result.getText();

                // 빈 값 제외
                if (!title.isEmpty()) {
                    // 결과 문자열 추가
                    sb.append(title).append("\n");
                }
            }
            
            // 결과 반환
            return sb.toString();

        } catch (Exception e) {
            return "Error: " + e.getMessage();
        } finally {
            // 브라우저 종료
            driver.quit();
        }

    }
}
