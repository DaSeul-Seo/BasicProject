package com.example.linkarchive.repository;

import org.springframework.data.jpa.repository.JpaRepository;

import com.example.linkarchive.entity.LinkTag;
import com.example.linkarchive.entity.Link;

public interface LinkTagRepository extends JpaRepository<LinkTag, Long> {
    void deleteByLink(Link link);
}
