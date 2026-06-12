<script>
    import fastapi from "../lib/api"
    import Error from "../components/Error.svelte"
    import { link, push } from 'svelte-spa-router'
    import { is_login, username, answer_page, sort_by, desc } from "../lib/store"
    import { marked } from 'marked'
    import moment from 'moment/min/moment-with-locales'
    moment.locale('ko')
    
    // Detail 컴포넌트 호출할 때 전달한 파라미터 값을 읽으려면 변수 선언
    export let params = {}
    let question_id = params.question_id
    // console.log('question_id' + question_id)

    let question = { answers:[], voter:[], content:'', question_comments: [] }
    let content = ""
    let error = {detail:[]}

    // 답변 페이징
    let answer_list = []
    let size = 3
    let total = 0

    // 댓글
    let is_question = true
    let question_comment_content = ''
    let answer_comment_content = ''
    let question_comment_modify_id = -1
    let answer_comment_modify_id = -1
    let question_edit_comment_content = ''
    let answer__edit_comment_content = ''
    let reply_target_id = -1
    let reply_content = ''

    $: total_page = Math.ceil(total/size)

    // 게시판 글 가져오기
    function get_question() {
        fastapi("get", "/api/question/detail/" + question_id, {}, (json) => {
            question = json
        });
    }

    get_question()

    // 답변 등록
    function post_answer(event) {
        event.preventDefault()
        let url = "/api/answer/create/" + question_id
        let params = {
            content: content
        }
        fastapi('post', url, params, (json) => {
            content = ''
            error = {detail:[]}

            $answer_page = 0    // 최신 답변 보이게 첫 답변페이지로 이동
            get_answer_list()   // 답변 리스트 다시 불러오기
            get_question()      // 총 질문 정보 갱신
        },
        (err_json) => {
            error = err_json
        })
    }
    
    // 질문 삭제
    function delete_question(_question_id) {
        if (window.confirm("정말로 삭제하시겠습니까?")){
            let url = "/api/question/delete"
            let params = {
                question_id: _question_id
            }

            fastapi('delete', url, params,
                (json) => {
                    push('/')
                },
                (err_json) => {
                    error = err_json
                }
            )
        }
    }

    // 답변 삭제
    function delete_answer(answer_id) {
        if (window.confirm('정말로 삭제하시겠습니까?')) {
            let url = "/api/answer/delete"
            let params = {
                answer_id: answer_id
            }

            fastapi('delete', url, params,
                (json) => {
                    // get_question()
                    get_answer_list()
                },
                (err_json) => {
                    error = err_json
                }
            )
        }
    }

    // 질문 추천
    function vote_question(_question_id) {
        if (window.confirm('정말로 추천하시겠습니까?')) {
            let url = "/api/question/vote"
            let params = {
                question_id: _question_id
            }

            fastapi('post', url, params,
                (json) => {
                    get_question()
                },
                (err_json) => {
                    error = err_json
                }
            )
        }
    }

    // 답변 추천
    function vote_answer(_answer_id) {
        if (window.confirm('정말로 추천하시겠습니까?')) {
            let url = "/api/answer/vote"
            let params = {
                answer_id: _answer_id
            }

            fastapi('post', url, params,
                (json) => {
                    // get_question()
                    get_answer_list()
                },
                (err_json) => {
                    error = err_json
                }
            )
        }
    }

    // 답변 리스트
    function get_answer_list() {
        let params = {
            question_id: question_id,
            page: $answer_page,
            size: size,
            sort_by: $sort_by,
            desc: $desc
        }

        fastapi('get', '/api/answer/list', params, (json) => {
            answer_list = json.answer_list
            total = json.total
        })
    }

    // 질문 댓글 작성
    function post_comment_question() {
        event.preventDefault()
        let url = "/api/comment/create/question/" + question_id
        let params = {
            content: question_comment_content
        }
        fastapi('post', url, params, (json) => {
            question_comment_content = ''
            error = {detail:[]}
            get_question()
        },
        (err_json) => {
            error = err_json
        })
    }

    // 답변 댓글 작성
    function post_comment_answer(_answer_id) {
        event.preventDefault()
        let url = "/api/comment/create/answer/" + _answer_id
        let params = {
            content: answer_comment_content
        }

        fastapi('post', url, params,
            (json) => {
                answer_comment_content = ''
                error = {detail:[]}
                get_answer_list()
            },
            (err_json) => {
                error = err_json
            }
        )
    }

    // 댓글 수정
    function update_comment(_comment_id, _comment_content){
        let url = '/api/comment/update'
        let params = {
            comment_id: _comment_id,
            content: _comment_content
        }

        fastapi('put', url, params,
            (json) => {
                // get_question()
                get_answer_list()
                stop_editing(true)
                stop_editing(false)
            },
            (err_json) => {
                error = err_json
            }
        )
    }

    // 댓글 삭제
    function delete_comment(_comment_id) {
        if (window.confirm('정말로 삭제하시겠습니까?')) {
            let url = "/api/comment/delete"
            let params = {
                comment_id: _comment_id
            }

            fastapi('delete', url, params,
                (json) => {
                    get_question()
                },
                (err_json) => {
                    error = err_json
                }
            )
        }
    }

    function start_editing(index, _content, is_question) {
        if (is_question) {
            question_comment_modify_id = index
            question_edit_comment_content = _content
        }
        else {
            answer_comment_modify_id = index
            answer_edit_comment_content = _content
        }
    }
    
    function stop_editing(is_question) {
        if (is_question) {
            question_edit_comment_content = ''
            question_comment_modify_id = -1
        }
        else {
            answer_edit_comment_content = ''
            answer_comment_modify_id = -1
        }
    }

    // 대댓글
    // function start_reply(comment_id) {
    //     if (reply_target_id == comment_id) {
    //         reply_target_id = -1
    //         reply_content = ''
    //     }
    //     else {
    //         reply_target_id = comment_id
    //         reply_content = ''
    //     }
    // }

    $: $answer_page, $sort_by, $desc, get_answer_list()
</script>

<!-- <h1>{question.subject}</h1>
<div>
    {question.content}
</div>
<ul>
    {#each question.answers as answer}
        <li>{answer.content}</li>
    {/each}
</ul> -->

<div class="container my-3">
    <!-- 질문 -->
    <h2 class="border-bottom py-2">{question.subject}</h2>
    <div class="card my-3">
        <div class="card-body">
            <!-- <div class="card-text" style="white-space: pre-line;">{question.content}</div> -->
            <div class="card-text">
                {@html marked.parse(question.content)}
            </div>
            <div class="d-flex justify-content-end">
                {#if question.modify_date}
                <div class="badge bg-light text-dark p-2 text-start mx-3">
                    <div class="mb-2">(수정됨)</div>
                    <div>{moment(question.modify_date).format("YYYY년 MM월 DD일 hh:mm a")}</div>
                </div>
                {/if}
                <div class="badge bg-light text-dark p-2 text-start">
                    <div class="mb-2">{ question.user ? question.user.username : "" }</div>
                    <div>{moment(question.create_date).format("YYYY년 MM월 DD일 hh:mm a")}</div>
                </div>
            </div>
            <div class="my-3">
                <button class="btn btn-sm btn-outline-secondary"
                    on:click="{vote_question(question.id)}">
                    추천
                    <span class="badge rounded-pill bg-success">{ question.voter.length }</span>
                </button>
                {#if question.user && $username == question.user.username}
                <a use:link href="/question-modify/{question_id}"
                    class="btn btn-sm btn-outline-secondary">수정</a>
                <button class="btn btn-sm btn-outline-secondary"
                    on:click={() => delete_question(question_id)}>삭제</button>
                {/if}
            </div>
        </div>
    </div>
    <div class="my-3">
        <Error error={error}></Error>
    </div>
    <!-- 댓글 목록 -->
    {#if question.question_comments.length > 0}
    <div>
        {#each question.question_comments as comment (comment.id)}
        <div class="card-body">
        {#if question_comment_modify_id == comment.id}
        <div>
            <input class="form-control mb-3" bind:value={question_edit_comment_content}
                disabled = "{$is_login ? '' : 'disabled'}"
                maxlength="500"
                placeholder="Edit comment">
            <button class="btn btn-sm btn-outline-secondary mb-3"
                on:click="{() => update_comment(comment.id, edit_comment_content)}">저장</button>
            <button class="btn btn-sm btn-outline-secondary mb-3"
                on:click="{() => stop_editing(true)}">취소</button>
        </div>
        {/if}
        {#if question_comment_modify_id != comment.id}
        <div class="border-top pt-2 pb-2">
            <div class="card-text" style="font-size: 0.75rem">{comment.content}
                <span class="text-muted fw-bold" style="font-size: 0.8rem"> - {comment.user.username}</span>
                {#if comment.modify_date}
                <span class="text-muted" style="font-size: 0.7rem">{moment(comment.modify_date).format("YYYY년 MM월 DD일 hh:mm a")}(수정됨)</span>
                {:else}
                <span class="text-muted" style="font-size: 0.7rem">{moment(comment.create_date).format("YYYY년 MM월 DD일 hh:mm a")}</span>
                {/if}
                {#if comment.user && $username == comment.user.username}
                <button on:click="{() => start_editing(comment.id, comment.content, true)}"
                    class="btn btn-link btn-sm p-0 ms-2"
                    style="--bs-btn-padding-y: .25rem; --bs-btn-padding-x: .5rem; --bs-btn-font-size: .6rem;">수정</button>
                <button on:click="{() => delete_comment(comment.id)}"
                    class="btn btn-link btn-sm p-0 ms-2"
                    style="--bs-btn-padding-y: .25rem; --bs-btn-padding-x: .5rem; --bs-btn-font-size: .6rem;">삭제</button>
                {/if}
            </div>
        </div>
        {/if}
        </div>
        {/each}
    </div>
    {/if}
    <!-- 댓글 입력 -->
    <div class="mb-3 mt-3" style="display: {$is_login ? '' : 'none'}">
        <input type="text" bind:value={question_comment_content} class="form-control form-control-sm">
        <!-- <textarea rows="1" bind:value={comment_content} class="form-control form-control-sm"></textarea> -->
        <button class="btn btn-link btn-sm p-0"
            on:click="{post_comment_question}">댓글 등록</button>
        <button class="btn btn-link btn-sm p-0"
            on:click="{() => stop_editing(true)}">취소</button>
    </div>
    <!-- 질문 목록으로 -->
    <button class="btn btn-secondary" on:click="{() => {push('/')}}">목록으로</button>
    <!-- 답변 버튼 -->
    <!-- <h5 class="border-bottom my-2 py-2">{question.answers.length}개의 답변이 있습니다.</h5> -->
    <div class="row">
        <div class="col-6">
            <h5 class="border-bottom my-2 py-2">{total}개의 답변이 있습니다.</h5>
        </div>
        <div class="col-6 d-flex justify-content-end align-items-center">
            <button class="btn me-2 btn-outline-primary {$sort_by == 'voter_count' ? 'active' : ''}"
                on:click="{() => {$sort_by = 'voter_count', $desc = true}}">
                추천순
            </button>
            <button class="btn me-2 btn-outline-primary {$sort_by == 'create_date' && $desc == true ? 'active' : ''}"
                on:click="{() => {$sort_by = 'create_date', $desc = true}}">
                최신순
            </button>
            <button class="btn btn-outline-primary {$sort_by == 'create_date' && $desc == false ? 'active' : ''}"
                on:click="{() => ($sort_by = 'create_date', $desc = false)}">
                오래된순
            </button>
        </div>
    </div>
    <!-- 답변 목록 -->
    <!-- {#each question.answers as answer} -->
    {#each answer_list as answer (answer.id)}
    <div class="card my-3">
        <div class="card-body">
            <!-- <div class="card-text" style="white-space: pre-line;">{answer.content}</div> -->
            <div class="card-text">
                {@html marked.parse(answer.content)}
            </div>
            <div class="d-flex justify-content-end">
                {#if answer.modify_date}
                <div class="badge bg-light text-dark p-2 text-start mx-3">
                    <div class="mb-2">(수정됨)</div>
                    <div>{moment(answer.modify_date).format("YYYY년 MM월 DD일 hh:mm a")}</div>
                </div>
                {/if}
                <div class="badge bg-light text-dark p-2 text-start">
                    <div class="mb-2">{ answer.user ? answer.user.username : "" }</div>
                    <div>{moment(answer.create_date).format("YYYY년 MM월 DD일 hh:mm a")}</div>
                </div>
            </div>
            <div class="my-3">
                <button class="btn btn-sm btn-outline-secondary"
                    on:click="{vote_answer(answer.id)}">
                    추천
                    <span class="badge rounded-pill bg-success">{ answer.voter.length }</span>
                </button>
                {#if answer.user && $username == answer.user.username}
                <a use:link href="/answer-modify/{answer.id}"
                    class="btn btn-sm btn-outline-secondary">수정</a>
                <button class="btn btn-sm btn-outline-secondary"
                    on:click={() => delete_answer(answer.id)}>삭제</button>
                {/if}
            </div>
            <!-- 답변 댓글 -->
            {#if answer.answer_comments.length > 0}
            <div class="mt-2">
                {#each answer.answer_comments as comment (comment.id)}
                <div class="border-top pt-2 pb-2">
                    <div style="font-size: 0.75em">{comment.content}
                        <span class="text-muted fw-bold"> - {comment.user.username}</span>
                        {#if comment.modify_date}
                        <span class="text-muted" style="font-size: 0.7rem">{moment(comment.modify_date).format("YYYY년 MM월 DD일 hh:mm a")}(수정됨)</span>
                        {:else}
                        <span class="text-muted" style="font-size: 0.7rem">{moment(comment.create_date).format("YYYY년 MM월 DD일 hh:mm a")}</span>
                        {/if}
                        {#if comment.user && $username == comment.user.username}
                        <button on:click="{() => start_editing(comment.id, comment.content, false)}"
                            class="btn btn-link btn-sm p-0 ms-2"
                            style="--bs-btn-padding-y: .25rem; --bs-btn-padding-x: .5rem; --bs-btn-font-size: .6rem;">수정</button>
                        <button on:click="{() => delete_comment(comment.id)}"
                            class="btn btn-link btn-sm p-0 ms-2"
                            style="--bs-btn-padding-y: .25rem; --bs-btn-padding-x: .5rem; --bs-btn-font-size: .6rem;">삭제</button>
                        {/if}
                    </div>
                </div>
                {/each}
            </div>
            {/if}
            <div class="mt-2" style="display: {$is_login ? '' : 'none'}">
                <input type="text" bind:value={answer_comment_content} class="form-control form-control-sm">
                <button class="btn btn-link btn-sm p-0"
                    on:click="{() => post_comment_answer(answer.id)}">댓글 등록</button>
                <button class="btn btn-link btn-sm p-0"
                    on:click="{() => stop_editing(false)}">취소</button>
            </div>
        </div>
    </div>
    {/each}
    <!-- 페이징 처리 시작 -->
    <ul class="pagination justify-content-center">
        <!-- 이전페이지 -->
        <!-- 이전 페이지가 없으면 비활성화 -->
        <li class="page-item {$answer_page <= 0 && 'disabled'}">
        <button class="page-link" on:click="{() => $answer_page--}">이전</button>
        </li>
        <!-- 페이지 번호 -->
        {#each Array(total_page) as _, loop_page}
        <!-- 선택 기준 좌우 5개씩 보이도록 -->
        {#if loop_page >= $answer_page - 5 && loop_page <= $answer_page + 5}
        <li class="page-item {loop_page == $answer_page && 'active'}">
        <button on:click="{() => $answer_page = loop_page}" class="page-link">{loop_page + 1}</button>
        </li>
        {/if}
        {/each}
        <!-- 다음페이지 -->
        <li class="page-item {$answer_page >= total_page - 1 && 'disabled'}">
        <button class="page-link" on:click="{() => $answer_page++}">다음</button>
        </li>
    </ul>
    <!-- 페이징 처리 끝 -->
    <!-- 답변 등록 -->
    <!-- alert창이 아닌 컴포넌트로 표시 -->
    <Error error={error}></Error>
    <form method="post" class="my-3">
        <div class="mb-3">
            <!-- textarea에 값을 추가하거나 변경할 때마다 content의 값도 자동으로 변경 -->
            <textarea rows="10" bind:value={content}
                disabled={$is_login ? "" : "disabled"} class="form-control"></textarea>
        </div>
        <!-- 답변 등록 누르면 post_answer 함수 호출, 답변들록 성공하면 content에 빈 문자열 대입 -->
        <input type="submit" value="답변등록" class="btn btn-primary {$is_login ? '' : 'disabled'}" on:click="{post_answer}">
    </form>
</div>

<!-- <style>
    textarea {
        width: 100%;
    }
    input[type=submit] {
        margin-top: 10px;
    }
</style> -->