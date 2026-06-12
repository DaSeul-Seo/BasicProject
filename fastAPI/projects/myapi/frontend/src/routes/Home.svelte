<!-- 답변 페이지네이션 : https://kante-kante.tistory.com/49 -->
<script>
  import { get } from "svelte/store";
  import Category from "../components/Category.svelte";

  // .env & api.js 파일 만든 후
  import fastapi from "../lib/api"
  import { link } from 'svelte-spa-router'
  import { page, is_login, keyword, category_id } from "../lib/store"
  import moment from 'moment/min/moment-with-locales'
  moment.locale('ko')

  let question_list = []

  // 페이지네이션 추가
  let size = 10
  // let page = 0 // 스토어 변수 대체
  let total = 0
  // 검색어
  let kw = ''
  // svelte에서 $를 붙이면 해당 변수는 반응형 변수가 된다.
  // => total 변수 값이 API 호출로 인해 값이 변하면 total_page 변수 값도 실시간으로 재 계산 된다.
  $: total_page = Math.ceil(total/size)

  function get_question_list(){    
    let params = {
      page: $page,
      size: size,
      keyword: $keyword,
      category_id: $category_id
    }

    fastapi('get', '/api/question/list', params, (json) => {
      question_list = json.question_list
      total = json.total
      kw = $keyword
    });
  }

  // function get_category_list() {
  //   fastapi('get', '/api/category/list', {}, (json) => {
  //     category_list = json
  //   })
  // }

  // get_category_list()

  // $page, $keyword 값이 변경되면 자동으로 get_question_list() 함수 실행
  // 때문에 get_question_list의 _page 매개변수 제거
  $: $page, $keyword, $category_id, get_question_list()
  // $: get_question_list($page)

  // 방법2
  // function get_question_list() {
  //   fastapi('get', '/api/question/list', {}, (json) => {
  //     question_list = json.question_list
  //   });
  // }

  // 방법1
  // function get_question_list() {
  //   fetch("http://localhost:8000/api/question/list").then((response) => {
  //     response.json().then((json) => {
  //       question_list = json
  //     });
  //   });
  // }

  // get_question_list()
</script>

<div class="my-3">
  <div class="row my-3">
    <div class="col-6">
      <a use:link href="/question-create"
        class="btn btn-primary {$is_login ? '' : 'disabled'}">질문 등록하기</a>
    </div>
    <div class="col-6">
      <div class="input-group">
        <input type="text" class="form-control" bind:value="{kw}">
        <button class="btn btn-outline-secondary" on:click="{() => {$keyword = kw, $page = 0}}">
          찾기
        </button>
      </div>
    </div>
  </div>
  <div>
    <span class="text-danger fw-bold">총 {total}개</span>
  </div>
  <table class="table">
    <thead>
      <tr class="text-center table-dark">
        <th>카테고리</th>
        <th>번호</th>
        <th style="width: 50%">제목</th>
        <th>글쓴이</th>
        <th>작성일시</th>
      </tr>
    </thead>
    <tbody>
      {#each question_list as question, i}
      <tr class="text-center">
        <td>{question.category ? question.category.subject : ""}</td>
        <td>{ total - ($page * size) - i }</td>
        <td class="text-start">
          <a use:link href="/detail/{question.id}">{question.subject}</a>
          <!-- 답변 갯수 표시 -->
          {#if question.answer_count > 0 }
          <span class="text-danger small mx-2">{question.answer_count}</span>
          {/if}
        </td>
        <td>{ question.user ? question.user.username : "" }</td> <!-- 글쓴이 -->
        <td>{moment(question.create_date).format("YYYY년 MM월 DD일 hh:mm a")}</td>
      </tr>
      {/each}
    </tbody>
  </table>
  <!-- 페이징 처리 시작 -->
  <ul class="pagination justify-content-center">
    <!-- 이전페이지 -->
    <!-- 이전 페이지가 없으면 비활성화 -->
    <li class="page-item {$page <= 0 && 'disabled'}">
      <!-- <button class="page-link" on:click="{() => get_question_list($page - 1)}">이전</button> -->
      <button class="page-link" on:click="{() => $page--}">이전</button>
    </li>
    <!-- 페이지 번호 -->
    {#each Array(total_page) as _, loop_page}
    <!-- 선택 기준 좌우 5개씩 보이도록 -->
    {#if loop_page >= $page - 5 && loop_page <= $page + 5}
    <li class="page-item {loop_page == $page && 'active'}">
      <!-- <button on:click="{() => get_question_list(loop_page)}" class="page-link">{loop_page + 1}</button> -->
      <button on:click="{() => $page = loop_page}" class="page-link">{loop_page + 1}</button>
    </li>
    {/if}
    {/each}
    <!-- 다음페이지 -->
    <li class="page-item {$page >= total_page - 1 && 'disabled'}">
      <!-- <button class="page-link" on:click="{() => get_question_list($page + 1)}">다음</button> -->
      <button class="page-link" on:click="{() => $page++}">다음</button>
    </li>
  </ul>
  <!-- 페이징 처리 끝 -->
  <!-- 질문 등록 -->
  <!-- <a use:link href="/question-create" class="btn btn-primary {$is_login ? '' : 'disabled'}">질문 등록</a> -->
</div>

<!-- <ul>
  {#each question_list as question}
    <li><a use:link href="/detail/{question.id}">{question.subject}</a></li>
  {/each}
</ul> -->

<!-- <script>
  let message;

  fetch("http://localhost:8000/hello").then((response) => {
    response.json().then((json) => {
      message = json.message;
    });
  });
</script>

<h1>{message}</h1> -->