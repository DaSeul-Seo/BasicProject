package com.example.crawling.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;

import com.example.crawling.dto.SiteUser;

public interface UserRepository extends JpaRepository<SiteUser, Long>{
    Optional<SiteUser> findByuserId(String userId);
    
}
