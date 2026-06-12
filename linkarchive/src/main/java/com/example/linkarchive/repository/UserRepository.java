package com.example.linkarchive.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;

import com.example.linkarchive.entity.SiteUser;

public interface UserRepository extends JpaRepository<SiteUser, Long> {
    Optional<SiteUser> findByUserId(String userId);
}
