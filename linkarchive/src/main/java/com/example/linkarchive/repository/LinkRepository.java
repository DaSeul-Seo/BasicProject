package com.example.linkarchive.repository;

import java.util.List;
import java.util.Optional;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import com.example.linkarchive.entity.Link;
import com.example.linkarchive.entity.SiteUser;

public interface LinkRepository extends JpaRepository<Link, Long> {

    // 최신순 정렬
    // default List<Link> findLatest() {
    //     return findAll(
    //         Sort.by(Sort.Direction.DESC, "id")
    //     );
    // }

    // 제목 검색
    // where title like '%keyword%'
    List<Link> findByTitleContaining(String keyword);

    /*
    select *
    from link
    where category_id = ?
    order by id desc
     */
    Page<Link> findByCategoryIdOrderByIdDesc(Long categoryId, Pageable pageable);

    long countByUser(SiteUser user);
    long countByUserAndCategoryId(SiteUser user, Long categoryId);

    // 즐겨찾기 갯수
    /*
    select count(*)
    from link
    where favorite = true
    */
    long countByUserAndFavoriteTrue(SiteUser user);
    long countByUserAndCategoryIdAndFavoriteTrue(SiteUser user, Long categoryId);

    // 읽음 갯수
    /*
    select count(*)
    from link
    where readAt is not null
     */
    long countByUserAndReadAtIsNotNull(SiteUser user);
    long countByUserAndCategoryIdAndReadAtIsNotNull(SiteUser user, Long categoryId);

    // 안읽음 갯수
    /*
    select count(*)
    from link
    where readAt is null
     */
    long countByUserAndReadAtIsNull(SiteUser user);
    long countByUserAndCategoryIdAndReadAtIsNull(SiteUser user, Long categoryId);

    /*
    select *
    from link
    where favorite = true
    */
    Page<Link> findByFavoriteTrue(SiteUser user, Pageable pageable);

    Page<Link> findByUser(SiteUser user, Pageable pageable);

    Page<Link> findByUserAndTitleContaining(SiteUser user, String keyword, Pageable pageable);

    Page<Link> findByUserAndCategoryIdOrderByIdDesc(SiteUser user, Long categoryId, Pageable pageable);

    // 즐겨찾기만
    Page<Link> findByUserAndFavoriteTrue(SiteUser user, Pageable pageable);
    Page<Link> findByUserAndFavoriteTrueAndTitleContaining(SiteUser user, String keyword, Pageable pageable);

    // 안읽음만
    Page<Link> findByUserAndReadAtIsNull(SiteUser user, Pageable pageable);
    Page<Link> findByUserAndReadAtIsNullAndTitleContaining(SiteUser user, String keyword, Pageable pageable);
    
    // 태그검색
    @Query("""
        SELECT DISTINCT L
        FROM Link L
        LEFT JOIN L.linkTags LT
        LEFT JOIN LT.tag T
        WHERE L.user = :user
        AND(
            LOWER(L.title) LIKE LOWER(CONCAT('%', :keyword, '%'))
            OR 
            LOWER(T.name) LIKE LOWER(CONCAT('%', :keyword, '%'))
        )
    """)
    Page<Link> searchWithTag(@Param("user") SiteUser user, @Param("keyword") String keyword, Pageable pageable);

    // 즐겨찾기 태그검색
    @Query("""
        SELECT DISTINCT L
        FROM Link L
        LEFT JOIN L.linkTags LT
        LEFT JOIN LT.tag T
        WHERE L.favorite = true
        AND(
            LOWER(L.title) LIKE LOWER(CONCAT('%', :keyword, '%'))
            OR 
            LOWER(T.name) LIKE LOWER(CONCAT('%', :keyword, '%'))
        )
    """)
    Page<Link> searchFavoriteWithTag(@Param("user") SiteUser user, @Param("keyword") String keyword, Pageable pageable);

    // 안읽음 태그검색
    @Query("""
        SELECT DISTINCT L
        FROM Link L
        LEFT JOIN L.linkTags LT
        LEFT JOIN LT.tag T
        WHERE L.readAt IS NULL
        AND(
            LOWER(L.title) LIKE LOWER(CONCAT('%', :keyword, '%'))
            OR 
            LOWER(T.name) LIKE LOWER(CONCAT('%', :keyword, '%'))
        )
    """)
    Page<Link> searchUnreadWithTag(@Param("user") SiteUser user, @Param("keyword") String keyword, Pageable pageable);

    // 검색에 카테고리 추가
    Page<Link> findByUserAndCategoryIdAndFavoriteTrue(SiteUser user, Long categoryId, Pageable pageable);

    Page<Link> findByUserAndCategoryIdAndReadAtIsNull(SiteUser user, Long categoryId, Pageable pageable);

    Page<Link> findByUserAndCategoryId(SiteUser user, Long categoryId, Pageable pageable);

    // 전체 검색(통합)
    @Query("""
       SELECT DISTINCT L
       FROM Link L
       LEFT JOIN L.linkTags LT
       LEFT JOIN LT.tag T
       WHERE L.user = :user
       
       AND (:categoryId IS NULL OR L.category.id = :categoryId)

       AND (:favoriteOnly = false OR L.favorite = true)

       AND (:unreadOnly = false OR L.readAt IS NULL)

       AND (
        :keyword IS NULL OR :keyword = '' OR
        L.title LIKE %:keyword% OR
        T.name LIKE %:keyword%)
    """)
    Page<Link> searchLinks(@Param("user") SiteUser user,
                        @Param("categoryId") Long categoryId,
                        @Param("favoriteOnly") boolean favoriteOnly,
                        @Param("unreadOnly") boolean unreadOnly,
                        @Param("keyword") String keyword,
                        Pageable pageable);

    // URL 중복 체크
    Optional<Link> findByUserAndUrl(SiteUser user, String url);

    // 회원 탈퇴
    void deleteByUser(SiteUser user);

    //===============================================

    // findByUser
    @Query("""
        SELECT L FROM Link L
        WHERE L.isPublic = true
        OR L.user = :user
    """)
    Page<Link> findVisibleLinks(@Param("user") SiteUser user, Pageable pageable);

    // 카테고리 + 공개/개인
    @Query("""
        SELECT L FROM Link L
        WHERE (L.isPublic = true OR L.user = :user)
        AND L.category.id = :categoryId
    """)
    Page<Link> findVisibleByCategory(@Param("user") SiteUser user,
                                    @Param("categoryId") Long categoryId,
                                    Pageable pageable);

    // 즐겨찾기 전체 기준
    @Query("""
        SELECT L FROM Link L
        WHERE (L.isPublic = true OR L.user = :user)
        AND L.favorite = true
    """)
    Page<Link> findFavorites(@Param("user") SiteUser user, Pageable pageable);

    // 읽지 않음
    @Query("""
        SELECT L FROM Link L
        WHERE (L.isPublic =true OR L.user = :user)
        AND L.readAt IS NULL
    """)
    Page<Link> findUnread(@Param("user") SiteUser user, Pageable pageable);
}
