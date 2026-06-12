package com.example.crawling.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;

import lombok.RequiredArgsConstructor;

@Controller
@RequiredArgsConstructor
public class MainController {
    
    @GetMapping("/")
    public String home() {
        return "redirect:/main";
    }

    @GetMapping("/main")
    public String main() {
        return "main";
    } 
}
