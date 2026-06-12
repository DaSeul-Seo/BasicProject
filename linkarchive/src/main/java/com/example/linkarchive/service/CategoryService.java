package com.example.linkarchive.service;

import java.util.List;

import org.springframework.stereotype.Service;

import com.example.linkarchive.entity.Category;
import com.example.linkarchive.repository.CategoryRepository;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class CategoryService {
    private final CategoryRepository categoryRepository;

    public List<Category> findAll() {
        return categoryRepository.findAll();
    }

}
