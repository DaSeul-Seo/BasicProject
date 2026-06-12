package com.example.linkarchive.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class LoginController {

    @GetMapping("/")
    public String main() {
        return "user/login";
    }
    
    @GetMapping("/login")
    public String login() {
        return "user/login";
    }
    
}
