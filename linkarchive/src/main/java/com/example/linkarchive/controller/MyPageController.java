package com.example.linkarchive.controller;

import java.security.Principal;

import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import com.example.linkarchive.dto.PasswordForm;
import com.example.linkarchive.entity.SiteUser;
import com.example.linkarchive.service.UserService;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;

@Controller
@RequiredArgsConstructor
@RequestMapping("/mypage")
public class MyPageController {

    private final UserService userService;

    @GetMapping
    public String mypage(Principal principal, Model model) {
        String userId = principal.getName();
        SiteUser user = userService.findByUserId(userId);

        model.addAttribute("user", user);

        return "user/mypage/index";
    }

    @GetMapping("/password")
    public String passwordForm(PasswordForm form) {
        return "user/mypage/password";
    }

    @PostMapping("/password")
    public String changePassword(PasswordForm form, Principal principal, RedirectAttributes redirectAttributes) {
        String userId = principal.getName();

        try {
            userService.changePassword(userId, form);

            redirectAttributes.addFlashAttribute("successMessage", "비밀번호가 변경되었습니다.");
            
            return "redirect:/mypage";
        }
        catch (IllegalArgumentException e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());

            return "redirect:/mypage/password";
        }
    }

    @PostMapping("/delete")
    public String deleteAccount(Principal principal, RedirectAttributes redirectAttributes) {
        String userId = principal.getName();
        userService.deleteUser(userId);
        SecurityContextHolder.clearContext();

        redirectAttributes.addFlashAttribute("successMessage", "회원탈퇴가 완료되었습니다.");
        return "redirect:/";
    }
}
