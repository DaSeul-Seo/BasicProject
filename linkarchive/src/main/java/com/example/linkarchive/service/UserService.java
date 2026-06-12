package com.example.linkarchive.service;

import com.example.linkarchive.repository.LinkRepository;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import com.example.linkarchive.dto.PasswordForm;
import com.example.linkarchive.entity.SiteUser;
import com.example.linkarchive.repository.UserRepository;

import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class UserService {

    private final LinkRepository linkRepository;
    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;

    public void signup(String userid, String password) {
        SiteUser user = SiteUser.builder()
                                .userId(userid)
                                .password(passwordEncoder.encode(password))
                                .build();
        
        userRepository.save(user);
    }

    public SiteUser getUser(Long id) {
        return userRepository.findById(id)
            .orElseThrow(() -> new IllegalArgumentException("User not found"));
    }

    public SiteUser findByUserId(String userId) {
        return userRepository.findByUserId(userId).orElseThrow();
    }

    @Transactional
    public void changePassword(String userId, PasswordForm form) {
        SiteUser user = userRepository.findByUserId(userId).orElseThrow();

        if (!passwordEncoder.matches(form.getCurrentPassword(), user.getPassword())) {
            throw new IllegalArgumentException("현재 비밀번호가 다릅니다.");
        }

        if (!form.getNewPassword().equals(form.getConfirmPassword())) {
            throw new IllegalArgumentException("새 비밀번호가 일치하지 않습니다.");
        }

        user.setPassword(passwordEncoder.encode(form.getNewPassword()));
    }

    @Transactional
    public void deleteUser(String userId) {
        SiteUser user = userRepository.findByUserId(userId).orElseThrow();

        linkRepository.deleteByUser(user);

        userRepository.delete(user);
    }
}
