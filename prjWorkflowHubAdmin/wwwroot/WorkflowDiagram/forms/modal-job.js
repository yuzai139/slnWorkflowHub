// API 基本地址
const baseAddress = "https://localhost:7146";

document.addEventListener("DOMContentLoaded", function () {
    // DOM 元素定義
    const category1 = document.getElementById("category1"); // 職業類別選單
    const subCategoryGroup = document.getElementById("subCategoryGroup"); // 職業選單
    const category2 = document.getElementById("category2"); // 顯示群組
    const jobGroup = document.getElementById("jobGroup"); // 顯示組別選單
    const category3 = document.getElementById("category3"); // 職業組別選單
    const submitJobButton = document.getElementById("submitJobButton"); // 確定按鈕
    const jobname = document.getElementById("jobName");
    const HiddenJobItem = document.getElementById("hiddenJobItem");
 


    // 初始狀態隱藏
    submitJobButton.style.display = "none";
    subCategoryGroup.style.display = "none";
    jobGroup.style.display = "none";

    // ===== 事件處理區 =====

    // category1 改變事件：當選擇職業類別後顯示 subCategoryGroup 並載入對應職業
    category1.addEventListener("change", function () {
        if (category1.value && category1.value != "請選擇") {
            subCategoryGroup.style.display = "block";
            loadJobsByClass(category1.value); // 根據選擇的職業類別載入對應職業
        } else {
            subCategoryGroup.style.display = "none";
            jobGroup.style.display = "none";
            category2.value = "";
            submitJobButton.style.display = "none"; // 隱藏確定按鈕
        }
    });

    // category2 改變事件：當選擇群組後顯示 jobGroup
    category2.addEventListener("change", function () {
        if (category2.value && category2.value != "請選擇") {
            jobGroup.style.display = "block";
            loadJobItemsByJob(category2.value);
        } else {
            jobGroup.style.display = "none";
            submitJobButton.style.display = "none"; // 隱藏確定按鈕
        }
    });

    // category3 改變事件：當選擇職業時顯示確定按鈕
    category3.addEventListener("change", function () {
        if (category3.value && category3.value != "請選擇") {
            submitJobButton.style.display = "block";
        } else {
            submitJobButton.style.display = "none";
        }
    });

    // ===== 函數區 =====

    // 初始化職業類別
    if (category1) {
        loadJobClasses(category1); // 加載職業類別選單
    }

    // 載入職業類別選單
    function loadJobClasses(selectElement) {
        fetch(`${baseAddress}/api/TSopJobClasses`)
            .then(response => response.ok ? response.json() : Promise.reject("無法載入職業類別"))
            .then(data => {
                selectElement.innerHTML = '<option>請選擇</option>';
                data.forEach(jobClass => {
                    const option = document.createElement("option");
                    option.value = jobClass.fJobClassId; // 使用 FJobClassId 作為選項值
                    option.textContent = jobClass.fJobClass; // 顯示 FJobClass 作為選項文本
                    selectElement.appendChild(option);
                });
            })
            .catch(error => console.error("Error loading job classes:", error));
    }

    // 根據職業類別載入對應職業細項
    function loadJobsByClass(jobClassId) {
        fetch(`${baseAddress}/api/TSopJobs/${jobClassId}`)
            .then(response => {
                console.log("Response status:", response.status); // 確認狀態碼是否為200
                return response.ok ? response.json() : Promise.reject("無法載入職業細項");
            })
            .then(data => {
                console.log("職業細項資料:", data); // 檢查返回的職業資料
                category2.innerHTML = '<option>請選擇</option>';
                data.forEach(sopJob => {
                    const option = document.createElement("option");
                    option.value = sopJob.fJobId; // 使用 FJobId 作為選項值
                    option.textContent = sopJob.fJob; // 顯示 FJob 作為選項文本
                    category2.appendChild(option);
                });
            })
            .catch(error => console.error("Error loading jobs:", error));
    }


    // 根據職業細項類別載入對應職業
    function loadJobItemsByJob(jobId) {
        fetch(`${baseAddress}/api/TSopJobItems/${jobId}`)
            .then(response => {
                console.log("Response status:", response.status); // 確認狀態碼是否為200
                return response.ok ? response.json() : Promise.reject("無法載入職業");
            })
            .then(data => {
                console.log("職業資料:", data); // 檢查返回的職業資料
                category3.innerHTML = '<option>請選擇</option>';
                data.forEach(sopJobItem => {
                    const option = document.createElement("option");
                    option.value = sopJobItem.fJobItemId; // 使用 FJobId 作為選項值
                    option.textContent = sopJobItem.fJobItem; // 顯示 FJob 作為選項文本
                    category3.appendChild(option);
                });
            })
            .catch(error => console.error("Error loading jobs:", error));
    }


    // 確定按鈕點擊事件
    submitJobButton.addEventListener("click", function () {
        // 獲取選定的職業名稱
        const selectedJobName = category3.options[category3.selectedIndex].text;
        const selectedJobValue = category3.options[category3.selectedIndex].value;

        // 確認選擇了有效的職業選項
        if (category3.value && category3.value !== "請選擇") {
            // 將選定的職業名稱顯示在 jobName span 元素中
            jobname.textContent = selectedJobName;
            HiddenJobItem.value = selectedJobValue;

            // 關閉彈出窗口 
            document.getElementById("closejobButton").click();
        } else {
            console.error("未選擇有效的職業選項");
        }
    });
});
