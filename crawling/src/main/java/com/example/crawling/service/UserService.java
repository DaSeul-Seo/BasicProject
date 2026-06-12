package com.example.crawling.service;

import java.util.Optional;

import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import com.example.crawling.DataNotFoundException;
import com.example.crawling.dto.SiteUser;
import com.example.crawling.repository.UserRepository;

import lombok.RequiredArgsConstructor;

@RequiredArgsConstructor
@Service
public class UserService {
    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    
    public SiteUser create(String userid, String password, String email) {
        SiteUser user = new SiteUser();
        user.setUserId(userid);
        user.setPassword(passwordEncoder.encode(password));
        user.setEmail(email);
        this.userRepository.save(user);
        return user;
    }

    public SiteUser getUser(String userId) {
        Optional<SiteUser> user = this.userRepository.findByuserId(userId);
        if (user.isPresent()) {
            return user.get();
        }
        else {
            throw new DataNotFoundException("user not found");
        }
    }
     
}
