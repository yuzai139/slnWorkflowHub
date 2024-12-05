document.addEventListener("DOMContentLoaded", function () {
    // API 基本地址
    const baseAddress = "https://localhost:7146";

    // 獲取 <select> 元素
    const companySizeSelect = document.getElementById("companySizeSelect");
    let isDataLoaded = false; // 記錄是否已加載數據

    // 確認 <select> 元素存在
    if (companySizeSelect) {
        // 監聽 <select> 元素的點擊事件
        companySizeSelect.addEventListener("click", createOption());
        
    }
});


function createOption() {

    const baseAddress = "https://localhost:7146";
    let isDataLoaded = false; // 記錄是否已加載數據
    // 如果數據已加載，則不再重新加載
    if (isDataLoaded) return;

    // 發送 GET 請求以獲取公司規模資料
    fetch(`${baseAddress}/api/TCompanySizes`, {
        method: "GET"
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.json();
        })
        .then(data => {
            console.log("Fetched data:", data); // 調試用

            // 清空選單，以免多次點擊重複加入選項
            companySizeSelect.innerHTML = '<option>請選擇</option>';

            // 將公司規模選項加入到 <select> 元素
            data.forEach(companySize => {
                // 使用小寫的 `fCompanySize`
                if (companySize && companySize.fCompanySize) {
                    const option = document.createElement("option");
                    option.value = companySize.fCompanySize; // 用 fCompanySize 作為選項值
                    option.textContent = companySize.fCompanySize; // 顯示的文本也是 fCompanySize
                    console.log("Option value:", option.value, "Option text:", option.textContent); // 調試
                    companySizeSelect.appendChild(option); // 添加到選單
                } else {
                    console.warn("缺少 fCompanySize 屬性:", companySize); // 顯示警告
                }
            });

            // 設定為已加載狀態
            isDataLoaded = true;


        
        })
        .catch(error => {
            console.error("Error fetching company sizes:", error);
            alert("無法載入公司規模選項，請稍後再試");
        });
}
