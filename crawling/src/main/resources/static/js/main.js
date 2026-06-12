// 검색 엔진
let selectedEngine = "google";
// 검색 결과 저장
let searchResults = [];
// 현재 페이지
let currentPage = 1;
// 페이지당 건수
const pageSize = 5;
// 선택된 목록 저장
const selectedLinks = new Set();
// 선택된 목록 저장2
const selectedItems = new Map();

// html 로딩완료
document.addEventListener("DOMContentLoaded", function() {
    // 전체 선택
    document.getElementById("check_all")
            .addEventListener("change", function() {
                const checked = this.checked;

                document.querySelectorAll(".result-check")
                        .forEach(function(item) {
                            item.checked = checked;
                        });
            });
    // 엔진 버튼들
    const engineButtons = document.querySelectorAll(".engine-btn");

    engineButtons.forEach(function(button) {
        button.addEventListener("click", function() {
            // active 제거
            engineButtons.forEach(function (btn) {
                btn.classList.remove("active");
            });

            // 현재 버튼 active 추가
            button.classList.add("active");

            // 선택 엔진 저장
            selectedEngine = button.dataset.engine;
            // console.log(selectedEngine);
        });
    });

    // 검색 버튼
    const searchButton = document.getElementById("btn_search");

    // 검색 클릭
    searchButton.addEventListener("click", async function() {
        // 검색어
        const keyword = document.getElementById("search_kw").value;

        if (!keyword) {
            alert("검색어를 입력해 주세요.");
            return;
        }

        // 요청 url 생성
        const requestUrl = `/${selectedEngine}?keyword=${encodeURIComponent(keyword)}`;
        // console.log(requestUrl);

        try {
            // fetch 요청
            const response = await fetch(requestUrl);

            // json 반환
            const data = await response.json();

            // 결과 영역
            const resultArea = document.getElementById("result_area");

            // 초기화
            resultArea.innerHTML = "";

            // 결과 없음
            if (data.length == 0) {
                resultArea.innerHTML = "<p>검색 결과 없음</p>";
                return;
            }

            console.log(data);

            // 검색 결과 저장
            searchResults = data;

            // 첫 페이지
            currentPage = 1;

            // 화면 출력
            renderPage();

            // 결과 반복
            // data.forEach(function (item) {
            //     const title = item.title || "";
            //     const link = item.link || "";
            //     const snippet = item.snippet || "";

            //     // html 생성
            //     const html = `
            //         <div class="card mt-2">
            //             <div class="card-body">
            //                 <div class="form-check mb-2">
            //                     <input class="form-check-input result-check"
            //                             type="checkbox"
            //                             value="${link}">
            //                 </div>
            //                 <h5>${title}</h5>
            //                 <a href="${link}" target="_blank">${link}</a>
            //                 <p class="mt-2">${snippet}</p>
            //             </div>
            //         </div>
            //     `;
                
            //     //resultArea.innerHTML + html;
            //     resultArea.insertAdjacentHTML("beforeend", html);
            // });
        }
        catch (error) {
            console.error(error);
            alert("검색 실패");
        }

    });
});

function renderPage() {
    const resultArea = document.getElementById("result_area");

    resultArea.innerHTML = "";

    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    const pageData = searchResults.slice(start, end);

    pageData.forEach(function(item) {
        const title = item.title || "";
        const link = item.link || "";
        const snippet = item.snippet || "";

        if (this.checked) {
            selectedLinks.add(this.value);
            selectedItems.set(link, {
                title: title,
                link: link,
                snippet: snippet
            });
        }
        else {
            selectedLinks.delete(this.value);
            selectedItems.delete(link);
        }
        console.log(selectedLinks);
        console.log(Array.from(selectedItems.values()));

        const html = `
            <div class="card mt-2">
                <div class="card-body">
                    <div class="d-flex">
                        <input
                            type="checkbox"
                            class="form-check-input result-check me-3"
                            value="${link}"
                            data-title="${title}"
                            data-link="${link}"
                            data-snippet="${snippet}"
                            ${selectedLinks.has(link) ? "checked" : ""}>
                        <div>
                            <h5>${title}</h5>
                            <a 
                                href="${link}" 
                                target="_blank"
                                data-bs-toggle="popover"
                                data-bs-trigger="hover"
                                title="${snippet}"
                                data-bs-content="${snippet}">
                                ${link}
                            </a>
                            <p class="mt-2">${snippet}</p>
                        </div>
                    </div>
                </div>
            </div>
        `;

        resultArea.insertAdjacentHTML("beforeend", html);
    });

    renderPagination();

    bindCheckboxEvents();
    
}

function renderPagination() {
    const pagination = document.getElementById("pagination");

    pagination.innerHTML = "";

    const totalPage = Math.ceil(searchResults.length / pageSize);

    let html = "";

    // 이전 버튼
    html += `
        <button
            class="btn btn-outline-secondary me-1"
            onclick="movePage(${currentPage - 1})"
            ${currentPage == 1 ? "disabled" : ""}>
        이전
        </button>
    `;

    for (let i = 1; i <= totalPage; i++) {
        html += `
            <button
                class="btn ${i == currentPage ? "btn-primary" : "btn-outline-primary"} me-1"
                onclick="movePage(${i})">
            ${i}
            </button>
        `;
    }

    html += `
        <button
            class="btn btn-outline-secondary"
            onclick="movePage(${currentPage + 1})"
            ${currentPage == totalPage ? "disabled" : ""}>
        다음
        </button>
    `;

    pagination.innerHTML = html;
}

function movePage(page) {
    const totalPage = Math.ceil(searchResults.length / pageSize);

    if (page < 1 || page > totalPage) {
        return;
    }

    currentPage = page;

    renderPage();
}

function bindCheckboxEvents() {
    document.querySelectorAll(".result-check")
            .forEach(function(checkbox) {
                checkbox.addEventListener("change", onCheckboxChanged);
            });
}

function onCheckboxChanged() {
    const title = this.dataset.title;
    const link = this.dataset.link;
    const snippet = this.dataset.snippet;

    if (this.checked) {
        selectedLinks.add(this.value);
        selectedItems.set(link, {
            title: title,
            link: link,
            snippet: snippet
        });
    }
    else {
        selectedLinks.delete(this.value);
        selectedItems.delete(link);
    }
    console.log(selectedLinks);
    console.log(Array.from(selectedItems.values()));

}