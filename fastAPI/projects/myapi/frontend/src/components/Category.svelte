<script>
    import fastapi from "../lib/api"
    import { page, category_id } from "../lib/store"

    let category_list = []

    function get_category_list() {
        fastapi('get', '/api/category/list', {}, (json) => {
            category_list = json
        })
    }

    get_category_list()

</script>

<div class="category-sidebar">
    <button
        class="category-item {$category_id == 0 ? 'active' : ''}"
        on:click="{() => {
            $category_id = 0
            $page = 0
        }}">전체</button>

    <!-- 카테고리 목록 -->
    {#each category_list as category}
        <button
            class="category-item {$category_id == category.id ? 'active' : ''}"
            on:click="{() => {
                $category_id = category.id
                $page = 0
            }}">{category.subject}</button>
    {/each}
</div>

<style>
    .category-sidebar {
        width: 220px;
        border: 1px solid #ddd;
        background: #f8f9fa;
    }

    .category-item {
        width: 100%;
        padding: 14px 20px;
        border: none;
        border-bottom: 1px solid #ddd;
        background: white;
        text-align: left;
        font-size: 15px;
        cursor: pointer;
    }

    .category-item:hover {
        background: #f1f1f1;
    }

    .category-item.active {
        background: #8c8c8c;
        color: white;
        font-weight: bold;
    }

</style>