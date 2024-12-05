// 等待 DOM 完全加載後運行 JavaScript，所以連結放在index.js(body的最後面)
document.addEventListener("DOMContentLoaded", function () {
    // 獲取 select 和 按鈕元素
    const sharePermissionSelect = document.getElementById("share-permission");
    const copyLinkButton = document.getElementById("copy-link-button");

    // 監聽 select 元素的變化事件
    sharePermissionSelect.addEventListener("change", function () {
        // 獲取當前選中的選項
        const selectedOption = sharePermissionSelect.value;

        // 判斷選中的選項是否為「全部人-檢視」或「全部人-編輯」
        if (selectedOption === "全部人-檢視" || selectedOption === "全部人-編輯") {
            // 顯示「複製分享連結」按鈕
            copyLinkButton.style.display = "inline-block";
        } else {
            // 隱藏「複製分享連結」按鈕
            copyLinkButton.style.display = "none";
        }
    });
});
