package com.example.linkarchive.service;

import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.stream.Collectors;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;  // Spring Bean에 등록
import org.springframework.transaction.annotation.Transactional;

import com.example.linkarchive.entity.Category;
import com.example.linkarchive.entity.Link;
import com.example.linkarchive.entity.LinkTag;
import com.example.linkarchive.entity.SiteUser;
import com.example.linkarchive.entity.Tag;
import com.example.linkarchive.repository.CategoryRepository;
import com.example.linkarchive.repository.LinkRepository;
import com.example.linkarchive.repository.LinkTagRepository;
import com.example.linkarchive.repository.TagRepository;
import com.example.linkarchive.repository.UserRepository;

import lombok.RequiredArgsConstructor;  // 생성자 주입 자동 생성

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;

@Service
@RequiredArgsConstructor
public class LinkService {

    private final LinkRepository linkRepository;
    private final CategoryRepository categoryRepository;
    private final UserRepository userRepository;
    private final LinkTagRepository linkTagRepository;
    private final TagRepository tagRepository;


    public boolean save(String url, Long categoryId, String tags, String memo, String userId) {

        try {
            SiteUser user = userRepository.findByUserId(userId).orElseThrow();
            
            String normalizedUrl = normalizeUrl(url);

            // 중복체크
            if (linkRepository.findByUserAndUrl(user, normalizedUrl).isPresent()) {
                return false;
            }
            
            Document doc = Jsoup.connect(url)
                                .userAgent("Mozilla/5.0")
                                .get();
            
            String title = doc.title();

            Element metaDescription = doc.selectFirst("meta[name=description]");

            // 썸네일
            Element ogImage = doc.selectFirst("meta[property=og:image]");

            String thumbnail = ogImage != null ? ogImage.attr("content") : "";

            String description = metaDescription != null ? metaDescription.attr("content") : "";

            Category category = categoryRepository.findById(categoryId).orElseThrow();

            Link link = Link.builder()
                            .url(normalizedUrl)
                            .title(title)
                            .description(description)
                            .thumbnail(thumbnail)
                            .category(category)
                            .memo(memo)
                            .user(user)
                            //.createdAt(LocalDateTime.now()) -> spring이 자동으로 넣어줌
                            .build();

            linkRepository.save(link);
            saveTags(link, tags);

            return true;
        }
        catch (Exception e) {
            throw new RuntimeException("URL 수집 실패", e);
        }   
    }

    private String normalizeUrl(String url) {
        url = url.trim();
        if (url.endsWith("/")) {
            url = url.substring(0, url.length() - 1);
        }

        return url;
    }

    // public void save(String url, Long categoryId, String tags, String memo, String userId) {
    //     try {
    //         Document doc = Jsoup.connect(url)
    //                             .userAgent("Mozilla/5.0")
    //                             .get();   
    //         String title = doc.title();
    //         Element metaDescription = doc.selectFirst("meta[name=description]");
    //         // 썸네일
    //         Element ogImage = doc.selectFirst("meta[property=og:image]");
    //         String thumbnail = ogImage != null ? ogImage.attr("content") : "";
    //         String description = metaDescription != null ? metaDescription.attr("content") : "";
    //         Category category = categoryRepository.findById(categoryId)
    //                                                 .orElseThrow();
    //         // 로그인한 유저 가져오기
    //         // Authentication auth = SecurityContextHolder.getContext().getAuthentication();
    //         // String userId = auth.getName();
    //         SiteUser user = userRepository.findByUserId(userId)
    //                                         .orElseThrow();
    //         Link link = Link.builder()
    //                         .url(url)
    //                         .title(title)
    //                         .description(description)
    //                         .thumbnail(thumbnail)
    //                         .category(category)
    //                         .memo(memo)
    //                         .user(user)
    //                         //.createdAt(LocalDateTime.now()) -> spring이 자동으로 넣어줌
    //                         .build();
    //         linkRepository.save(link);
    //         saveTags(link, tags);
    //     }
    //     catch (Exception e) {
    //         throw new RuntimeException("URL 수집 실패", e);
    //     }
    // }

    public Page<Link> getLinks(SiteUser user, int page, String filter, String keyword, String sort, Long categoryId) {
        Sort sortObj;

        switch (sort) {
            case "oldest":
                sortObj = Sort.by("createdAt").ascending();
                break;
            case "favorite":
                sortObj = Sort.by("favorite").descending()
                                .and(Sort.by("createdAt").descending());
                break;
            default:
                sortObj = Sort.by("createdAt").descending();
                break;
        }
        // 0페이지, 1개씩, 최신순
        // Pageable pageable = PageRequest.of(page, 10, Sort.by("id").descending());
        Pageable pageable = PageRequest.of(page, 10, sortObj);

        boolean favoriteOnly = "favorite".equals(filter);
        boolean unreadOnly = "unread".equals(filter);

        // boolean hasKeyword = keyword != null && !keyword.isBlank();
        // boolean hasCategory = categoryId != null;

        return linkRepository.searchLinks(user, categoryId, favoriteOnly, unreadOnly, keyword, pageable);

        // switch (filter) {
        //     case "favorite":
        //         if (hasCategory) {
        //             if (hasKeyword) {
        //                 // return linkRepository.findByUserAndFavoriteTrueAndTitleContaining(user, keyword, pageable);
        //                 // return linkRepository.searchFavoriteWithTag(user, keyword, pageable);
        //                 return linkRepository.searchFavoriteWithTagAndCategory(user, keyword, categoryId, pageable);
        //             }
        //             return linkRepository.findByUserAndCategoryIdAndFavoriteTrue(user, categoryId, pageable);
        //         }
        //         if (hasKeyword) {
        //             return linkRepository.searchFavoriteWithTag(user, keyword, pageable);
        //         }
        //         return linkRepository.findByUserAndFavoriteTrue(user, pageable);
        //     case "unread":
        //         if (hasCategory) {
        //             if (hasKeyword) {
        //                 return linkRepository.searchUnreadWithTagAndCategory(user, keyword, categoryId, pageable);
        //             }
        //             return linkRepository.findByUserAndCategoryIdAndReadAtIsNull(user, categoryId, pageable);
        //         }
        //         if (hasKeyword) {
        //             // return linkRepository.findByUserAndReadAtIsNullAndTitleContaining(user, keyword, pageable);
        //             return linkRepository.searchUnreadWithTag(user, keyword, pageable);
        //         }
        //         return linkRepository.findByUserAndReadAtIsNull(user, pageable);
        //     default:
        //         if (hasCategory) {
        //             if (hasKeyword) {
        //                 return linkRepository.searchWithTagAndCategory(user, keyword, categoryId, pageable);
        //             }
        //             return linkRepository.findByUserAndCategoryId(user, categoryId, pageable);
        //         }
        //         if (hasKeyword) {
        //             // return linkRepository.findByUserAndTitleContaining(user, keyword, pageable);
        //             return linkRepository.searchWithTag(user, keyword, pageable);
        //         }
        //         return linkRepository.findByUser(user, pageable);
        // }
        // return linkRepository.findAll(pageable);
        //return linkRepository.findByUserAndTitleContaining(user, keyword, pageable);
    }

    public Page<Link> findAll(int page, String filter, String userId) {
        // 정렬
        Sort sortOption;

        switch (filter) {
            case "favorite":
                sortOption = Sort.by("favorite")
                                .descending()
                                .and(Sort.by("id").descending());
                break;
            case "unread":
                sortOption = Sort.by("readAt")
                                .ascending();
                break;
            default:
                sortOption = Sort.by("id").descending();
                break;
        }

        // 0페이지, 1개씩, 최신순
        // Pageable pageable = 
        //     PageRequest.of(page, 1, Sort.by("id").descending());
        Pageable pageable = 
            PageRequest.of(page, 10, sortOption);

        SiteUser user = userRepository.findByUserId(userId)
                                        .orElseThrow();

        // return linkRepository.findAll(pageable);
        return linkRepository.findByUser(user, pageable);
    }

    // 제목 검색
    public Page<Link> search(String keyword, String userId, int page, String filter) {
        // return linkRepository.findByTitleContaining(keyword);

        Sort sortOption;

        switch (filter) {
            case "favorite":
                sortOption = Sort.by("favorite")
                                .descending()
                                .and(Sort.by("id").descending());
                break;
            case "unread":
                sortOption = Sort.by("readAt")
                                .ascending();
                break;
            default:
                sortOption = Sort.by("id").descending();
                break;
        }

        SiteUser user = userRepository.findByUserId(userId)
                                        .orElseThrow();

        Pageable pageable = 
            PageRequest.of(page, 10, sortOption);
        return linkRepository.findByUserAndTitleContaining(user, keyword, pageable);
    }

    // 즐겨찾기 토글
    @Transactional
    public boolean toggleFavorite(Long id) {
        Link link = linkRepository
            .findById(id)
            .orElseThrow(() -> new RuntimeException("링크를 찾을 수 없습니다."));

        link.setFavorite(!link.isFavorite());

        linkRepository.save(link);
        return link.isFavorite();
        /* save를 안하는 이유
        @Transactional 안에서 Entity를 수정하면 set만 해줘도 트랜잭션 종료 시점에서
        update link
        set favorite = true
        where id = ?
        자동 실행 (Dirty Checking)
         */
    }

    // 읽음 처리
    @Transactional
    public void toggleRead(Long id) {
        Link link = linkRepository
            .findById(id)
            .orElseThrow(() -> new RuntimeException("링크를 찾을 수 없습니다."));

        if (link.getReadAt() == null) {
            link.setReadAt(LocalDateTime.now());
        }
        else {
            link.setReadAt(null);
        }
    }

    // 삭제
    @Transactional
    public void delete(Long id) {
        // 존재 여부 확인
        Link link = linkRepository
            .findById(id)
            .orElseThrow(() -> new RuntimeException("링크를 찾을 수 없습니다."));

        linkRepository.delete(link);
    }

    public Page<Link> findByCategory(Long categoryId, Pageable pageable, String userId) {
        // return linkRepository.findByCategoryIdOrderByIdDesc(categoryId, pageable);
        SiteUser user = userRepository.findByUserId(userId)
                                        .orElseThrow();
        return linkRepository.findByUserAndCategoryIdOrderByIdDesc(user, categoryId, pageable);
    }

    public Page<Link> findFavorites(int page, String userId) {
        SiteUser user = userRepository.findByUserId(userId)
                                        .orElseThrow();
        Pageable pageable = PageRequest.of(page, 10, Sort.by("id").descending());
        return linkRepository.findByFavoriteTrue(user, pageable);
    }

    // 전체 갯수
    public long getTotalCount(SiteUser user, Long categoryId) {
        if (categoryId == null) {
            return linkRepository.countByUser(user);
        }
        return linkRepository.countByUserAndCategoryId(user, categoryId);
    }

    // 즐겨찾기 갯수
    public long getFavoriteCount(SiteUser user, Long categoryId) {
        if (categoryId == null) {
            return linkRepository.countByUserAndFavoriteTrue(user);
        }
        return linkRepository.countByUserAndCategoryIdAndFavoriteTrue(user, categoryId);
    }

    // 읽음 갯수
    public long getReadCount(SiteUser user, Long categoryId) {
        if (categoryId == null) {
            return linkRepository.countByUserAndReadAtIsNotNull(user);
        }
        return linkRepository.countByUserAndCategoryIdAndReadAtIsNotNull(user, categoryId);
    }

    // 안읽음 갯수
    public long getUnreadCount(SiteUser user, Long categoryId) {
        if (categoryId == null) {
            return linkRepository.countByUserAndReadAtIsNull(user);
        }
        return linkRepository.countByUserAndCategoryIdAndReadAtIsNull(user, categoryId);
    }

    public String getRelativeTime(LocalDateTime dateTime) {
        long hours = ChronoUnit.HOURS.between(dateTime, LocalDateTime.now());

        if (hours < 24) {
            return hours + "시간 전";
        }

        long days = ChronoUnit.DAYS.between(dateTime, LocalDateTime.now());
        return days + "일 전";
    }

    // 태그 저장
    public void saveTags(Link link, String tags) {
        if (tags == null || tags.isBlank()) return;

        String[] tagNames = tags.split(",");
        
        for(String tagName : tagNames) {
            String newTagName = tagName.trim();

            if (newTagName.isBlank()) continue;

            Tag tag = tagRepository.findByName(newTagName)
                                    .orElseGet(() -> {
                                        Tag newTag = new Tag();
                                        newTag.setName(newTagName);

                                        return tagRepository.save(newTag);
                                    });

            LinkTag linkTag = new LinkTag();
            linkTag.setLink(link);
            linkTag.setTag(tag);

            linkTagRepository.save(linkTag);
        }
    }

    public Link findById(Long id, String userId) {
        Link link = linkRepository.findById(id)
                            .orElseThrow(() -> new RuntimeException("링크없음"));

        if (!link.getUser().getUserId().equals(userId)) {
            throw new RuntimeException("권한없음");
        }

        return link;
    }

    public String getTagNames(Link link) {
        return link.getLinkTags()
                    .stream()
                    .map(linkTag -> linkTag.getTag().getName())
                    .collect(Collectors.joining(", "));
    }

    @Transactional
    public void update(Long id, String title, String url, String tags, String memo, Long categoryId) {
        Link link  = linkRepository.findById(id).orElseThrow();

        Category category = categoryRepository.findById(categoryId).orElseThrow();

        link.setTitle(title);
        link.setUrl(url);
        link.setMemo(memo);
        link.setCategory(category);

        updateTags(link, tags);
    }

    @Transactional
    public void updateTags(Link link, String tags) {
        linkTagRepository.deleteByLink(link);

        saveTags(link, tags);
    }

    // 읽음 처리
    @Transactional
    public Link markAsReadAndGet(Long id) {
        Link link = linkRepository.findById(id).orElseThrow();

        if (link.getReadAt() == null) {
            link.setReadAt(LocalDateTime.now());
        }

        return link;
    }
}
