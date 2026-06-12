import qs from "qs"
import { access_token, username, is_login } from "./store"
import { get } from 'svelte/store'
import { push } from 'svelte-spa-router'

// operation : 데이터를 처리하는 방법, 소문자만 사용 (get, post, put, delete)
// url : 요청 URL, 단 백엔드 서버의 호스트명 이후의 URL만 전달 (/api/question/list)
// params : 요청 데이터 ({page: 1, keyword: "마크다움"})
// success_callback : API 호출 성공시 수행할 함수, 전달된 함수에는 API 호출시 리턴되는 json이 입력으로 주어짐
// failure_callback : API 호출 실패시 수행할 함수, 전달된 함수에는 오류 값이 입력으로 주어짐
const fastapi = (operation, url, params, success_callback, failure_callback) => {
    let method = operation
    let content_type = 'application/json'
    let body = JSON.stringify(params)

    // 로그인일 경우 method, content_type, body값 설정
    if (operation == 'login'){
        method = 'post'
        content_type = 'application/x-www-form-urlencoded'
        body = qs.stringify(params)
    }

    // let _url = 'http://localhost:8000' + url
    // .env 파일에 명시해주었기에 가능
    // import 하고 띄어쓰면 안됨
    let _url = import.meta.env.VITE_SERVER_URL + url
    
    if (method == 'get') {
        _url += "?" + new URLSearchParams(params)
    }

    let options = {
        method: method,
        headers: {
            "Content-Type": content_type
        }
    }

    // 질문 등록, 답변 등록 API에 인증이 적용되었기에 API 호출시 HTTP 헤더에 액세스 토큰을 담아서 호출해야 함
    // fastapi 함수는 $access_token 불가 => svelte 컴포넌트만 가능 => get, set 사용해야 함
    const _access_token = get(access_token)
    if (_access_token) {
        // Bearer 하고 띄어쓰기 필수!중요!조심!
        options.headers["Authorization"] = "Bearer " + _access_token
    }

    if (method != 'get') {
        options['body'] = body
    }

    fetch(_url, options)
        .then(response => {
            if (response.status == 204) {   // No content
                if (success_callback) {
                    success_callback()
                }
                return
            }
            response.json()
                .then(json => {
                    if (response.status >= 200 && response.status < 300) {
                        if (success_callback) {
                            success_callback(json)
                        }
                    }
                    else if (operation != 'login' && response.status == 401) { // 토큰 timeout
                        access_token.set('')
                        username.set('')
                        is_login.set(false)
                        alert("로그인이 필요합니다.")
                        push('/user-login')
                    }
                    else {
                        if (failure_callback) {
                            failure_callback(json)
                        }
                        else {
                            alert(JSON.stringify(json))
                        }
                    }
                })
                .catch(error => {
                    alert(JSON.stringify(error))
                });
        });
}

export default fastapi