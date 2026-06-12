package com.example.linkarchive.config;

import org.springframework.boot.CommandLineRunner;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import com.example.linkarchive.entity.Category;
import com.example.linkarchive.repository.CategoryRepository;

@Configuration
public class DataInitializer {
    
    @Bean
    CommandLineRunner init(CategoryRepository repository) {
        return args -> {
            if (repository.count() == 0) {
                repository.save(Category.builder()
                                        .name("개발")
                                        .build());

                repository.save(Category.builder()
                                        .name("DB")
                                        .build());

                repository.save(Category.builder()
                                        .name("AWS")
                                        .build());

                repository.save(Category.builder()
                                        .name("투자")
                                        .build());
                
                repository.save(Category.builder()
                                        .name("기타")
                                        .build());
            }
        };
    }
}
