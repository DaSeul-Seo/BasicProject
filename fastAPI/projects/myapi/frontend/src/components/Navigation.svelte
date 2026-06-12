<script>
    import { link } from 'svelte-spa-router'
    import { page, access_token, username, is_login, keyword, answer_page, sort_by, desc } from "../lib/store"
</script>

<!-- 네비게이션바 -->
<nav class="navbar navbar-expand-lg navbar-light bg-light border-bottom">
    <div class="container-fluid">
        <a use:link class="navbar-brand" href="/" on:click="{() => { $keyword = '', $page = 0, $answer_page = 0, $sort_by = 'create_date', $desc = true }}">Pybo</a>
        <button
            class="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#navbarSupportedContent"
            aria-controls="navbarSupportedContent"
            aria-expanded="false"
            aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                { #if $is_login }
                <li class="nav-item">
                    <!-- 로그아웃 눌렀을 때 토큰정보 초기화 -->
                    <a use:link class="nav-link" href="/user-login" on:click={() => {
                        $access_token = ''
                        $username = ''
                        $is_login = false
                    }}>로그아웃 ({$username})</a>
                </li>
                {:else}
                <li class="nav-item">
                    <a use:link class="nav-link" href="/user-create">회원가입</a>
                </li>
                <li class="nav-item">
                    <a use:link class="nav-link" href="/user-login">로그인</a>
                </li>
                {/if}
            </ul>
        </div>
    </div>
</nav>