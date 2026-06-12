package com.example.linkarchive.service;

import java.util.List;

import org.springframework.stereotype.Service;

import com.example.linkarchive.dto.TagCountDto;
import com.example.linkarchive.entity.SiteUser;
import com.example.linkarchive.repository.TagRepository;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class TagService {

    private final TagRepository tagRepository;

    public List<TagCountDto> getTagCounts(SiteUser user) {
        return tagRepository.findTagCounts(user);
    }
}
