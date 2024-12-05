// API 基本地址
const baseAddress = "https://localhost:7146";
document.addEventListener("DOMContentLoaded", function () {
    const IndustryCate = document.getElementById("IndustryCate");
    const industryGroup = document.getElementById("industryGroup"); // 確認是否成功獲取
    const Industry = document.getElementById("industry");
    const submitIndButton = document.getElementById("submitIndustry");
    const industryName = document.getElementById("industryName");
    const HiddenIndustry = document.getElementById("hiddenIndustry");

    // 檢查是否成功取得 industryGroup
    if (!industryGroup) {
        console.error("無法找到 element with id 'industryGroup'");
        return;
    }

    // 初始隱藏行業確定按鈕
    submitIndButton.style.display = "none";

    IndustryCate.addEventListener("change", function () {
        if (IndustryCate.value) {
            industryGroup.style.display = "block";
            Industry.value = "";
            submitIndButton.style.display = "none";
            loadIndustryByClass(IndustryCate.value);
        } else {
            industryGroup.style.display = "none";
            submitIndButton.style.display = "none";
        }
    });

    Industry.addEventListener("change", function () {
        if (Industry.value && Industry.value != "請選擇") {
            submitIndButton.style.display = "block";
        } else {
            submitIndButton.style.display = "none";
        }
    });

    function clickIndustryOk(industry) {
        if (industry != null)
            industryName.value = industry;
    };

    // ===== 函數區 =====

    // 初始化職業類別
    if (IndustryCate) {
        loadIndustryClass(IndustryCate); // 加載職業類別選單
    }

    // 載入類別選單
    function loadIndustryClass(selectElement) {
        fetch(`${baseAddress}/api/TSopIndustryClasses`)
            .then(response => response.ok ? response.json() : Promise.reject("無法載入行業類別"))
            .then(data => {
                selectElement.innerHTML = '<option>請選擇</option>';
                data.forEach(industryClass => {
                    const option = document.createElement("option");
                    option.value = industryClass.fIndustryClassId; // 選項值
                    option.textContent = industryClass.fIndustryClass; // 選項文本
                    selectElement.appendChild(option);
                });
            })
            .catch(error => console.error("Error loading:", error));
    }

    // 根據行業類別載入行業
    function loadIndustryByClass(industryClassId) {
        fetch(`${baseAddress}/api/TSopIndustries/${industryClassId}`)
            .then(response => {
                console.log("Response status:", response.status); // 確認狀態碼是否為200
                return response.ok ? response.json() : Promise.reject("無法載入行業");
            })
            .then(data => {
                console.log("行業資料:", data); // 檢查返回的職業資料
                Industry.innerHTML = '<option>請選擇</option>';
                data.forEach(industry => {
                    const option = document.createElement("option");
                    option.value = industry.fIndustryId; // 選項值
                    option.textContent = industry.fIndustry; // 選項文本
                    Industry.appendChild(option);
                });
            })
            .catch(error => console.error("Error loading:", error));
    }

    // 確定按鈕點擊事件
    submitIndButton.addEventListener("click", function () {
        // 獲取選定的職業名稱
        const selectedIndustryName = industry.options[industry.selectedIndex].text;
        const selectIndustryValue = industry.options[industry.selectedIndex].value;

        // 確認選擇了有效的選項
        if (industry.value && industry.value !== "請選擇") {
            // 將選定的行業名稱顯示在 jobName span 元素中
            industryName.textContent = selectedIndustryName;
            HiddenIndustry.value = selectIndustryValue; //把選定的行業Id顯示再畫面元素中

            // 關閉彈出窗口 
            document.getElementById("closeIndustryButton").click();
        } else {
            console.error("未選擇有效的行業選項");
        }
    });

});
