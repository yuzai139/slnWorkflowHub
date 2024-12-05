document.addEventListener("DOMContentLoaded", function () {
    // 取得 HTML 元素的參考( Member Sop )
    const sopNameInput = document.getElementById("inputSopName"); // 工作流程名稱
    const sopDepartmentInput = document.getElementById("sopDepartment"); // 部門
    const sopCustomerInput = document.getElementById("sopCustomer"); // 服務對象
    const sopBusinessInput = document.getElementById("sopBusiness"); // 營運模式
    const sopDescriptionTextarea = document.getElementById("sopDescription"); // 工作流程說明

    const fakeInfoButton = document.getElementById("fake-info"); // 按鈕

    if (fakeInfoButton) {
        // 綁定點擊事件到假資料按鈕
        fakeInfoButton.addEventListener("click", function () {
            // 設定每個輸入框的假資料
            sopNameInput.value = "法務人員案件處理與保險合規流程"; // 假的工作流程名稱
            sopDepartmentInput.value = "法務部"; // 假的部門名稱
            sopCustomerInput.value = "處理保險合同糾紛，提供法律建議與支持"; // 假的服務對象描述
            sopBusinessInput.value = "確保保險業務流程符合法律與政策要求"; // 假的營運模式描述

            // 使用 Template Literals 設定多行文字
            sopDescriptionTextarea.value = `步驟 1：案件接洽與背景調查
- 接收保險糾紛案件或法律需求，進行初步背景調查。
- 『評估案件的法律風險』與解決途徑，制定初步方案。

步驟 2：法律研究與方案制定
- 進行相關法規與政策研究，搜集支持材料。
- 起草法律意見書或解決方案，與相關部門確認。

步驟 3：合同審查與修改
- 審查保險合同條款，確保內容符合法律要求。
- 提出修改建議，優化合同風險管理條款。

步驟 4：合規檢查與內部審核
- 對保險業務流程進行合規檢查，出具審核報告。
- 與合規部門協作，完善保險業務操作細則。

步驟 5：爭議解決與法庭代理
- 代表公司參與爭議解決，包括調解與法庭辯護。
- 協助處理客戶投訴與理賠糾紛，確保合法性。

步驟 6：總結與改進
- 收集案件經驗與合規檢查數據，提出改進建議。
- 制定培訓計劃，提升法律團隊的專業能力與效率。`;
        });
    }
});
