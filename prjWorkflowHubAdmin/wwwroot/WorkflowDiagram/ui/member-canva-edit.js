


const baseAddress = "https://localhost:7146";


//=========================   綁定畫面元素 + 載入畫面資料  =========================
document.addEventListener("DOMContentLoaded", function () {
    const SopId = document.getElementById("sopId").value;

    // 綁定 HTML 元素
    const sopTypeElement = document.getElementById("sopType");
    const sopMemberIdElement = document.getElementById("sopMemberId");
    const diagramImagePath = document.getElementById("diagramImagePath");
    const hiddenJobItemId = document.getElementById("hiddenJobItem");
    const hiddenIndustryId = document.getElementById("hiddenIndustry");
    const sopNameDisplay = document.getElementById("sopName");
    const sopNameInput = document.getElementById("inputSopName");
    const jobNameDisplay = document.getElementById("jobName");
    const industryNameDisplay = document.getElementById("industryName");
    const sopDepartmentInput = document.getElementById("sopDepartment");
    const sopCustomerInput = document.getElementById("sopCustomer");
    const sopBusinessInput = document.getElementById("sopBusiness");
    const sharePermissionSelect = document.getElementById("share-permission");
    const sopFileStatusSelect = document.getElementById("sopFileStatus");
    const sopEditTimeDisplay = document.getElementById("sopEditTime");
    const sopDescriptionTextarea = document.getElementById("sopDescription");
    const companySizeSelect = document.getElementById("companySizeSelect");


    // 調用 API 並將數據更新到頁面
    fetch(`${baseAddress}/api/TMmbSopCreate/${SopId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("API 請求失敗，狀態碼：" + response.status);
            }
            return response.json();
        })
        .then(data => {
            console.log("API 回應資料：", data);

            // 更新隱藏欄位的值
            sopTypeElement.value = data.FSopType || '';
            sopMemberIdElement.value = data.FMemberId || '';
            hiddenJobItemId.value = data.FJobItemId || '';
            hiddenIndustryId.value = data.FIndustryId || '';

            // 更新顯示的元素值
            sopNameDisplay.textContent = data.FSopName || '';
            sopNameInput.value = data.FSopName || '';
            jobNameDisplay.textContent = data.JobItem || '';
            industryNameDisplay.textContent = data.Industry || '';

            // 其他資料欄位的更新
            sopDepartmentInput.value = data.FDepartment || '';
            sopCustomerInput.value = data.FCustomer || '';
            sopBusinessInput.value = data.FBusiness || '';
            sharePermissionSelect.value = data.FSharePermission || '';
            sopFileStatusSelect.value = data.FFileStatus || '';
            sopEditTimeDisplay.textContent = data.FEditTime || '';
            sopDescriptionTextarea.value = data.FSopDescription || '';

            // 設置公司規模下拉選單的值
            companySizeSelect.value = data.FCompanySize || '';

            //圖片路徑
            diagramImagePath.value = data.FSopFlowImagePath;

            ////公司規模的API打完之後，再傳一次到畫面上
            //let a = document.getElementById("hiddenCompanySize").value;
            //companySizeSelect.value = a;

            // 加載圖片（若存在）
            if (diagramImagePath.value) {
                loadDiagramImage(diagramImagePath.value);
            } else {
                console.log("沒有找到可加載的流程圖。");
            }
        })
        .catch(error => {
            console.error("載入 SOP 資料時發生錯誤:", error);
        });

});


// 將圖片加載到 Canvas 的函數
async function loadDiagramImage(imagePath) {
    const fullImageUrl = `${window.location.origin}/Workflow/SopImages/${imagePath}`;
    try {
        const response = await fetch(fullImageUrl);
        if (!response.ok) {
            throw new Error("無法加載圖片，請檢查圖片路徑或伺服器響應。");
        }

        const blob = await response.blob(); // 將響應轉換為 Blob 文件
        const file = new File([blob], imagePath, { type: 'image/png' }); // 創建一個 File 對象
        const canvas = document.querySelector('#canvas'); // 獲取 canvas 元素

        // 調用 loadData 方法將圖片載入到畫布
        await loadData(canvas, file);
    } catch (error) {
        console.error("無法加載圖片，請檢查圖片路徑或伺服器響應：", error);
        
    }
}


