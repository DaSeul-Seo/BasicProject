import { writable } from "svelte/store";
// store 변수
// 3페이지 글을 확인하고 뒤로가기하면 3페이지가 아닌 1페이지로 다시 로딩된다.
// 상세페이지 호출 시 현재 질문 목록의 페이지 번호를 전달하기 위해 스토어 변수 활용

// 스토어 변수의 지속성
const persist_storage = (key, initValue) => {
    const storedValueStr = localStorage.getItem(key)
    const store = writable(storedValueStr != null ? JSON.parse(storedValueStr) : initValue)
    // subscribe : 스토어에 저장된 값이 변경될 때 실행되는 콜백 함수
    // => 스토어 변수의 값이 변경될 때 localStorage 값도 함께 변경
    store.subscribe((val) => {
        localStorage.setItem(key, JSON.stringify(val))
    })
    return store
}

// export const page = writable(0)
export const page = persist_storage("page", 0)
export const access_token = persist_storage("access_token", "")
export const username = persist_storage("username", "")
export const is_login = persist_storage("is_login", false)
export const keyword = persist_storage("keyword", "")

// 답변 페이징
export const answer_page = persist_storage("answer_page", 0)
export const sort_by = persist_storage("sort_by", "create_date")
export const desc = persist_storage("desc", true)

// 카테고리
export const category_id = writable(0)