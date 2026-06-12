package com.example.linkarchive.controller;

import java.security.Principal;

import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;

import com.example.linkarchive.dto.SignupForm;
import com.example.linkarchive.entity.SiteUser;
import com.example.linkarchive.service.UserService;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;

@Controller
@RequiredArgsConstructor
@RequestMapping("/user")
public class UserController {
    
    private final UserService userService;

    @GetMapping("/signup")
    public String signupForm() {
        return "user/signup";
    }

    @PostMapping("/signup")
    public String signup(SignupForm form) {
        userService.signup(form.getUserId(), form.getPassword());

        return "redirect:/login";
    }

    @GetMapping("/{id}")
    public String getUser(@PathVariable Long id, Model model) {
        SiteUser user = userService.getUser(id);
        model.addAttribute("user", user);
        return "user/detail";
    }

}
