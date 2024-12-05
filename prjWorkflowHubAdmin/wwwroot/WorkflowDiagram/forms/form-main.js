///* 加載其他js檔案 */

///**
// * loadScript - 一個函數，用來動態加載指定的 JavaScript 檔案
// * @param {string} url - 要加載的 JavaScript 檔案的路徑
// */
//function loadScript(url) {
//    // 創建一個新的 <script> 元素，這是用來在 HTML 中引用 JavaScript 檔案的元素
//    const script = document.createElement('script');
    
//    // 設置 <script> 元素的 src 屬性，將其指向我們要加載的 JS 檔案的路徑
//    script.src = url;

//    // 設置 <script> 的 type 屬性為 "text/javascript"，這是標準的 JavaScript MIME 類型
//    // 雖然在現代瀏覽器中這個屬性是默認的，但明確設置它是個好習慣
//    script.type = 'text/javascript';

//    // 將這個 <script> 元素添加到 <body> 元素的最後，這樣網頁會在此時加載並執行該 JS 檔案
//    document.body.appendChild(script);
//}

///**
// * 動態加載其他 JS 檔案
// * 這裡依次加載 js檔案
// * 當這些函數被調用時，`loadScript` 會創建 <script> 標籤並加載對應的檔案
// */
//document.addEventListener('DOMContentLoaded', function() {
//    // DOM 已經完全加載後，開始動態加載 JS 檔案
//    /* 連結位址要從html檔案開始算 */
//    loadScript('~/WorkflowDiagram/forms/publisher-form.js');
//    loadScript('~/WorkflowDiagram/forms/form-commond.js');
//});
import './publisher-form.js';
import './form-common.js'; 
