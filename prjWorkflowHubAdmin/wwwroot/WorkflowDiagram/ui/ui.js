/** @type {HTMLDivElement} */ let overlay; 
// 定義一個變量 overlay，用來存儲覆蓋層的 HTMLDivElement

/** @param {boolean} isDisable */
// 定義 uiDisable 函數，接受一個布爾值 isDisable，控制覆蓋層的顯示與隱藏
export function uiDisable(isDisable) {
    if (isDisable && !overlay) {
        overlay = document.createElement('div');  
        // 當 isDisable 為 true 且 overlay 未定義時，創建一個新的 div 作為覆蓋層
        overlay.style.cssText = 'z-index: 2; position: fixed; left: 0; top: 0; width:100%; height:100%; background: #fff; opacity: 0';
        // 設置覆蓋層的 CSS 樣式，覆蓋整個屏幕並設定透明度為 0
        overlay.innerHTML =
        `<style>
        @keyframes blnk {
            0% { opacity: 0; }
            50% { opacity: 0.7; }
            100% { opacity: 0; }
        }
        .blnk { animation: blnk 1.6s linear infinite; }
        </style>`;
        // 使用內嵌的 CSS 定義 blnk 動畫，實現閃爍效果，並將這段樣式應用到 .blnk class
        overlay.classList.add('blnk'); 
        // 將 .blnk class 添加到覆蓋層，讓其開始閃爍
        document.body.append(overlay); 
        // 將覆蓋層插入到頁面中
    } else if (!isDisable) {
        overlay.remove(); 
        // 如果 isDisable 為 false，則移除覆蓋層
        overlay = null; 
        // 重置 overlay 變量
    }
}

/** @param {boolean} show */
// 定義 tipShow 函數，控制畫布的指針事件和提示信息的顯示
export function tipShow(show) {
    document.getElementById('diagram').style.pointerEvents = show ? 'none' : 'unset';
    // 當 show 為 true，禁用畫布的指針事件，否則恢復
    document.getElementById('tip').style.display = show ? 'unset' : 'none';
    // 控制提示信息的顯示與隱藏
}