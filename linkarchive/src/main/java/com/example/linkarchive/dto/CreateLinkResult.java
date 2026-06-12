package com.example.linkarchive.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
public class CreateLinkResult {
    private boolean success;
    private String message;
}
