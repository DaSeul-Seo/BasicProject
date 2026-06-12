package com.example.linkarchive.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
public class TagCountDto {
    private String name;
    private Long count;
}
