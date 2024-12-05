
////=========================   綁定畫面元素 + 載入畫面資料 -Publisher =========================
//function getViewDatabyPubSop() {

//    // 綁定Publisher獨有的 HTML 元素
//    const publisherId = document.getElementById("publisherId");
//    const pubContent = document.getElementById("pubContent");
//    const sopReleaseStatus = document.getElementById("releaseStatus");
//    const isRelease = document.getElementById("isRelease");
//    const sopPrice = document.getElementById("sopPrice");
//    const sopSalePoints = document.getElementById("salePoints");
//    const releaseTime = document.getElementById("sopReleaseTime");

//    //更新Publisher獨有的欄位
//    releaseTime.textContent = data.FReleaseTime || '';
//    sopReleaseStatus.value = data.FReleaseStatus || '';
//    publisherId.value = data.FPublisherId || '';
//    pubContent.value = data.FPubContent || '';
//    isRelease.value = data.FIsRelease || '';
//    sopPrice.value = data.FPrice || '';
//    sopSalePoints.value = data.FSalePoints || '';


//    // 更新隱藏欄位的值
//    sopTypeElement.value = data.FSopType || '';
//    sopMemberIdElement.value = data.FMemberId || '';
//    publisherId = data.FPublisherId || '';
//    hiddenJobItemId.value = data.FJobItemId || '';
//    hiddenIndustryId.value = data.FIndustryId || '';

//    // 更新顯示的元素值
//    sopNameDisplay.textContent = data.FSopName || '';
//    sopNameInput.value = data.FSopName || '';
//    jobNameDisplay.textContent = data.JobItem || '';
//    industryNameDisplay.textContent = data.Industry || '';

//    // 其他資料欄位的更新
//    sopDepartmentInput.value = data.FDepartment || '';
//    sopCustomerInput.value = data.FCustomer || '';
//    sopBusinessInput.value = data.FBusiness || '';
//    sopDescriptionTextarea.value = data.FSopDescription || '';

//    // 設置公司規模下拉選單的值
//    companySizeSelect.value = data.FCompanySize || '';

//    //圖片路徑
//    diagramImagePath.value = data.FSopFlowImagePath;

//    // 加載圖片（若存在）
//    if (diagramImagePath.value) {
//        loadDiagramImage(diagramImagePath.value);
//    } else {
//        console.log("沒有找到可加載的流程圖。");
//    }


//}
