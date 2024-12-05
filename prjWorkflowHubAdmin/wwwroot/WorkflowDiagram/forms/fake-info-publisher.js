document.addEventListener("DOMContentLoaded", function () {
    // 取得 HTML 元素的參考( Publisher Sop )
    const sopPubContentnTextarea = document.getElementById("pubContent"); // 發布內容
    const sopPrice = document.getElementById("sopPrice"); // 價格
    const sopPoints = document.getElementById("salePoints"); // 點數
    const fakePublishInfoButton = document.getElementById("fake-info-publisher"); // 假的Publish按鈕

    if (fakePublishInfoButton) {
        // 綁定點擊事件到假資料按鈕
        fakePublishInfoButton.addEventListener("click", function () {
            // 設定每個輸入框的假資料
            sopPrice.value = 3600; 
            sopPoints.value = 600;
            // 使用 Template Literals 設定多行文字
            sopPubContentnTextarea.value = `商品特色：

詳細涵蓋法務人員從案件管理到保險合規檢查的完整流程。
適合保險業，提升法務工作效率與合規保障能力。
提供專業文檔（Word、PDF、PPT）格式，便於學習與應用。

適用對象：
(1)初入行法務人員：快速掌握保險業法務工作的技能與流程。
(2)資深法務專員：提升案件處理效率與合規檢查能力。
(3)保險公司與法律部門：標準化工作流程，降低法律風險。

價值點：
a. 幫助法務人員高效完成案件管理與合規檢查工作，減少法律風險。
b. 標準化流程確保保險業務的合規性與準確性。
c. 增強保險公司與客戶之間的信任與合作關係。`;
        });
    }
});
