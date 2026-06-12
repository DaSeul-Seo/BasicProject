package com.example.linkarchive.controller;

import com.example.linkarchive.repository.TagRepository;
import java.security.Principal;
import java.util.HashMap;
import java.util.Map;

import org.springframework.data.domain.Page;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import com.example.linkarchive.entity.Link;
import com.example.linkarchive.entity.SiteUser;
import com.example.linkarchive.service.CategoryService;
import com.example.linkarchive.service.LinkService;
import com.example.linkarchive.service.TagService;
import com.example.linkarchive.service.UserService;

import lombok.RequiredArgsConstructor;

@Controller
@RequiredArgsConstructor
@RequestMapping("/links")
public class LinkController {
    
    private final TagRepository tagRepository;
    private final LinkService linkService;
    private final CategoryService categoryService;
    private final UserService userService;
    private final TagService tagService;

    @GetMapping
    public String list(@RequestParam(name = "page", defaultValue = "0") int page, 
                        @RequestParam(name = "filter", defaultValue = "all") String filter,
                        @RequestParam(name = "keyword", required = false) String keyword,
                        @RequestParam(name = "sort", defaultValue = "latest") String sort,
                        @RequestParam(name = "categoryId", required = false) Long categoryId,
                        Principal principal,
                        Model model) {

        String userId = principal.getName();
        SiteUser user = userService.findByUserId(userId);
        
        Page<Link> links = linkService.getLinks(user, page, filter, keyword, sort, categoryId);
        model.addAttribute("links", links);
        model.addAttribute("filter", filter);
        model.addAttribute("keyword", keyword);
        model.addAttribute("categoryId", categoryId);
        model.addAttribute("loginUser", userId);
        
        model.addAttribute("categories", categoryService.findAll());
        // model.addAttribute("tags", tagRepository.findAll());
        model.addAttribute("tags", tagService.getTagCounts(user));
        model.addAttribute("sort", sort);
        
        model.addAttribute("totalCount", linkService.getTotalCount(user, categoryId));
        model.addAttribute("favoriteCount", linkService.getFavoriteCount(user, categoryId));
        model.addAttribute("readCount", linkService.getReadCount(user, categoryId));
        model.addAttribute("unreadCount", linkService.getUnreadCount(user, categoryId));

        return "link/list";
    }

    @PostMapping
    public String save(@RequestParam(name = "url") String url,
                        @RequestParam(name = "categoryId") Long categoryId,
                        @RequestParam(name = "tags", required = true) String tags,
                        @RequestParam(name = "memo") String memo,
                        Principal principal,
                        RedirectAttributes redirectAttributes) {
        
        String userSeqId = principal.getName();

        boolean success = linkService.save(
            url,
            categoryId,
            tags,
            memo,
            userSeqId);

        if (success) {
            redirectAttributes.addFlashAttribute("successMessage", "링크가 저장되었습니다.");
        }
        else {
            redirectAttributes.addFlashAttribute("errorMessage", "이미 저장된 링크입니다.");
        }

        return "redirect:/links";

    }

    // 검색
    // @GetMapping("/search")
    // public String search (@RequestParam(name = "page", defaultValue = "0") int page, 
    //                         @RequestParam(name = "sort", defaultValue = "latest") String sort,
    //                         @RequestParam(name = "keyword") String keyword, 
    //                         Principal principal,
    //                         Model model) {         
    //     String userId = principal.getName();
    //     SiteUser user = userRepository.findByUserId(userId).orElseThrow();
    //     Page<Link> links = linkService.search(keyword, userId, page, sort);
    //     model.addAttribute("links", links);
    //     model.addAttribute("keyword", keyword); 
    //     model.addAttribute("sort", sort);     
    //     model.addAttribute("categories", categoryService.findAll());
    //     model.addAttribute("totalCount", linkService.getTotalCount(user));
    //     model.addAttribute("favoriteCount", linkService.getFavoriteCount(user));
    //     model.addAttribute("readCount", linkService.getReadCount(user));
    //     model.addAttribute("unreadCount", linkService.getUnreadCount(user));
    //     model.addAttribute("loginUser", userId);
    //     return "link/list";
    // }

    // 즐겨찾기
    /* @PathVariable : /links/3/favorite 요청이 오면 id가 3이 됨. */
    @PostMapping("/{id}/favorite")
    public String favorite (@PathVariable(name = "id") Long id,
                            @RequestParam(name = "page", defaultValue = "0") int page, 
                            @RequestParam(name = "filter", defaultValue = "all") String filter,
                            @RequestParam(name = "keyword", required = false) String keyword,
                            @RequestParam(name = "sort", defaultValue = "latest") String sort,
                            @RequestParam(name = "categoryId", required = false) Long categoryId,
                            RedirectAttributes redirectAttributes) {
        linkService.toggleFavorite(id);

        redirectAttributes.addAttribute("page", page);
        redirectAttributes.addAttribute("filter", filter);
        redirectAttributes.addAttribute("keyword", keyword);
        redirectAttributes.addAttribute("sort", sort);

        if (categoryId != null) {
            redirectAttributes.addAttribute("categoryId", categoryId);
        }

        return "redirect:/links";
    }

    // 즐겨찾기 ajax
    @PostMapping("/{id}/favorite/ajax")
    @ResponseBody
    public Map<String, Object> favoriteAjax(@PathVariable("id") Long id) {
        boolean favorite = linkService.toggleFavorite(id);

        Map<String, Object> result = new HashMap<>();
        result.put("favorite", favorite);
        
        return result;
    }

    // 읽음 처리
    @PostMapping("/{id}/read")
    public String read (@PathVariable(name = "id") Long id,
                        @RequestParam(name = "page", defaultValue = "0") int page, 
                        @RequestParam(name = "filter", defaultValue = "latest") String filter,
                        @RequestParam(name = "keyword", required = false) String keyword) {
        linkService.toggleRead(id);
        return "redirect:/links?page" + page
                + "&filter=" + filter
                + "&keyword=" + keyword;
    }

    // 삭제
    @PostMapping("/{id}/delete")
    public String delete (@PathVariable(name = "id") Long id,
                            @RequestParam(name = "page", defaultValue = "0") int page, 
                            @RequestParam(name = "filter", defaultValue = "latest") String filter,
                            @RequestParam(name = "keyword", required = false) String keyword) {
        linkService.delete(id);
        return "redirect:/links?page" + page
                + "&filter=" + filter
                + "&keyword=" + keyword;
    }

    @PostMapping("/{id}/delete-ajax")
    public String deleteAjax(@PathVariable(name = "id") Long id,
                            @RequestParam(name = "page", defaultValue = "0") int page, 
                            @RequestParam(name = "filter", defaultValue = "latest") String filter,
                            @RequestParam(name = "keyword", required = false) String keyword) {
        linkService.delete(id);
        return "redirect:/links?page" + page
                + "&filter=" + filter
                + "&keyword=" + keyword;
    }

    // 카테고리
    // @GetMapping("/category/{categoryId}")
    // public String category (@PathVariable(name = "categoryId") Long categoryId,
    //                         @RequestParam(name = "page", defaultValue = "0") int page,
    //                         Principal principal,
    //                         Model model) {
    //     String userId = principal.getName();
    //     Pageable pageable = PageRequest.of(page, 10, Sort.by("id").descending());
    //     model.addAttribute("links", linkService.findByCategory(categoryId, pageable, userId));
    //     model.addAttribute("categories", categoryService.findAll());
    //     model.addAttribute("loginUser", userId);
    //     return "link/list";
    // }

    // 즐겨찾기 상단 고정
    // @GetMapping("/favorites")
    // public String favorites(@RequestParam(name = "page", defaultValue = "0") int page,
    //                         Principal principal,
    //                         Model model) {
    //     String userId = principal.getName();
    //     SiteUser user = userRepository.findByUserId(userId).orElseThrow();
    //     model.addAttribute("links", linkService.findFavorites(page, userId));
    //     model.addAttribute("categories", categoryService.findAll());
    //     model.addAttribute("totalCount", linkService.getTotalCount(user));
    //     model.addAttribute("favoriteCount", linkService.getFavoriteCount(user));
    //     model.addAttribute("readCount", linkService.getReadCount(user));
    //     model.addAttribute("unreadCount", linkService.getUnreadCount(user));
    //     model.addAttribute("loginUser", userId);
    //     return "link/favorites";
    // }

    // 수정
    @GetMapping("/{id}/edit")
    public String editForm(@PathVariable(name = "id") Long id,
                            @RequestParam(name = "page", defaultValue = "0") int page,
                            @RequestParam(name = "keyword", required = false) String keyword,
                            @RequestParam(name = "filter", defaultValue = "all") String filter,
                            @RequestParam(name = "sort", defaultValue = "latest") String sort,
                            @RequestParam(name = "categoryId", required = false) Long categoryId,
                            Principal principal,
                            Model model) {

        String userId = principal.getName();
        Link link = linkService.findById(id, userId);

        model.addAttribute("link", link);
        model.addAttribute("categories", categoryService.findAll());
        model.addAttribute("tagNames", linkService.getTagNames(link));
        model.addAttribute("page", page);
        model.addAttribute("keyword", keyword);
        model.addAttribute("filter", filter);
        model.addAttribute("sort", sort);
        model.addAttribute("categoryId", categoryId);

        return "link/edit";
    }

    @PostMapping("/{id}/edit")
    public String edit(@PathVariable(name = "id") Long id,
                        @RequestParam(name = "title") String title,
                        @RequestParam(name = "url") String url,
                        @RequestParam(name = "tags") String tags,
                        @RequestParam(name = "memo") String memo,
                        @RequestParam(name = "page", defaultValue = "0") int page,
                        @RequestParam(name = "keyword", required = false) String keyword,
                        @RequestParam(name = "filter", defaultValue = "all") String filter,
                        @RequestParam(name = "sort", defaultValue = "latest") String sort,
                        @RequestParam(name = "categoryId", required = false) Long categoryId,   // 수정
                        @RequestParam(name = "selectedCategoryId", required = false) Long selectedCategoryId,   // 검색 유지
                        RedirectAttributes redirectAttributes) {
        
        linkService.update(id, title, url, tags, memo, categoryId);

        redirectAttributes.addFlashAttribute("successMessage", "링크가 수정되었습니다.");
        // 수정 완료 후 원래 위치 복귀
        redirectAttributes.addAttribute("page", page);
        redirectAttributes.addAttribute("keyword", keyword);
        redirectAttributes.addAttribute("filter", filter);
        redirectAttributes.addAttribute("sort", sort);

        if (categoryId != null) {
            redirectAttributes.addAttribute("categoryId", selectedCategoryId);
        }

        return "redirect:/links";
    }

    @GetMapping("/tags")
    public String tags(Model model) {
        model.addAttribute("tags", tagRepository.findAll());

        return "tag/list";
    }

    // 읽음 처리
    @GetMapping("/{id}/visit")
    public String visit(@PathVariable(name = "id") Long id) {
        Link link = linkService.markAsReadAndGet(id);

        return "redirect:" + link.getUrl();
    }

}
