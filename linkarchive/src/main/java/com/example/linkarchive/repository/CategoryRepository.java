package com.example.linkarchive.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;

import com.example.linkarchive.entity.Category;

public interface CategoryRepository extends JpaRepository<Category, Long> {
    Optional<Category> findByName(String name);
}
