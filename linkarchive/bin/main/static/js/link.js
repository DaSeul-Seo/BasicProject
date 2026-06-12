const csrfToken = document.querySelector('meta[name="_csrf"]').content;
const csrfHeader = document.querySelector('meta[name="_csrf_header"]').content;

document.addEventListener("DOMContentLoaded", function() {
    // 카드 클릭
    document.querySelectorAll(".link-card")
        .forEach(card => {
            card.addEventListener("click", function (e) {
                if (e.target.closest("button") ||
                    e.target.closest("form") ||
                    e.target.closest("a")) {
                        return;
                    }

                
                const id = this.dataset.id;
                
                if (this.dataset.read == "false") {

                    this.dataset.read = "true";
                    
                    this.classList.add("opacity-50");

                    const badge = this.querySelector(".read-badge");
    
                    if (badge) {
                        badge.textContent = "읽음";
                        badge.classList.remove("bg-danger");
                        badge.classList.add("bg-success");
                    }
                }
                
                window.open(`/links/${id}/visit`, "_blank");

                // const url = this.dataset.url;
                // if (url) {
                //     window.open(url, "_blank");
                // }
            });
        });
    // 삭제 확인
    document.querySelectorAll(".delete-btn")
        .forEach(btn => {
            btn.addEventListener("click", deleteLink);
        });
    
    // 즐겨찾기 ajax
    document.querySelectorAll(".favorite-btn")
        .forEach(btn => {
            btn.addEventListener("click", toggleFavorite);
        });

    // Toast
    const toastEl = document.getElementById("successToast");

    if (toastEl) {
        const toast = new bootstrap.Toast(toastEl, {
            delay: 3000
        });

        toast.show();
    }
});

async function deleteLink(e) {
    e.preventDefault();

    if (!confirm("정말 삭제하시겠습니까?")) {
        return;
    }

    const btn = e.currentTarget;
    const id = btn.dataset.id;

    // const csrfToken = document.querySelector('meta[name="_csrf"]').content;
    // const csrfHeader = document.querySelector('meta[name="_csrf_header"]').content;

    const response = await fetch(`/links/${id}/delete-ajax`, {
        method: "POST",
        headers: {
            [csrfHeader]: csrfToken
        }
    });

    if (response.ok) {
        showToast("삭제되었습니다.");

        const card = document.querySelector(`[data-link-id="${id}"]`);
        const cardWrapper = card.closest(".col-md-6")

        // 사라지는 애니메이션
        cardWrapper.classList.add("fade-out");
        
        // 제거
        setTimeout(() => {
            cardWrapper.remove();
        }, 300);
    }
}

async function toggleFavorite(e) {
    e.preventDefault();

    const button = e.currentTarget;
    const id = button.dataset.id;

    // const csrfToken = document.querySelector('meta[name="_csrf"]').content;
    // const csrfHeader = document.querySelector('meta[name="_csrf_header"]').content;

    try {
        const response = await fetch(
            `/links/${id}/favorite/ajax`, {
                method: "POST",
                headers: {
                    [csrfHeader]: csrfToken
                }
            }
        );

        const data = await response.json();

        button.querySelector("span").textContent = data.favorite ? "★" : "☆";

        showToast(data.favorite ? "즐겨찾기에 추가되었습니다." : "즐겨찾기에서 제거되었습니다.");
    }
    catch(error) {
        console.error(error);
        alert("즐겨찾기 처리 중 오류가 발생했습니다.");
    }
}

// Toast
function showToast(message) {
    const toastContainer = document.getElementById("toast-container");

    if (!toastContainer) return;

    const toastHtml = `
        <div class="toast text-bg-success border-0 mb-2" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button"
                        class="btn-close btn-close-white me-2 m-auto"
                        data-bs-dismiss="toast">
                </button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML("beforeend", toastHtml);

    const toastEl = toastContainer.lastElementChild;

    const toast = new bootstrap.Toast(
        toastEl,
        { delay: 3000 }
    );

    toast.show();

    toastEl.addEventListener("hidden.bs.toast", () => {
        toastEl.remove();
    });
}