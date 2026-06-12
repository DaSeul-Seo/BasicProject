package com.example.linkarchive.repository;

import java.util.List;
import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import com.example.linkarchive.dto.TagCountDto;
import com.example.linkarchive.entity.SiteUser;
import com.example.linkarchive.entity.Tag;


public interface TagRepository extends JpaRepository<Tag, Long> {

    Optional<Tag> findByName(String name);

    @Query("""
        select new com.example.linkarchive.dto.TagCountDto(
            lt.tag.name,
            count(lt)
        )
        from LinkTag lt
        where lt.link.user = :user
        group by lt.tag.name
        order by count(lt) desc
    """)
    List<TagCountDto> findTagCounts(@Param("user") SiteUser user);
    
}