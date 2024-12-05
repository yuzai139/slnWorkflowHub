/* 附件上傳-開始 */

// 為文件上傳的 <input> 元素添加 'change' 事件監聽器
// 當用戶選擇了文件時觸發此事件
document.getElementById('file-upload').addEventListener('change', function(event) {
    
    // 獲取 class 為 'file-list' 的元素，用於顯示上傳文件的列表
    const fileList = document.querySelector('.file-list');

    // 從事件對象中獲取上傳的文件，event.target.files 是一個類似陣列的 FileList 對象
    const files = event.target.files;

    // 迭代所有上傳的文件
    for (let i = 0; i < files.length; i++) {
        const file = files[i];  // 獲取當前文件對象

        // 為每個文件創建一個新的 <div> 元素，用來顯示文件名稱和刪除按鈕
        const fileItem = document.createElement('div');
        fileItem.classList.add('file-item');  // 添加 'file-item' class 以便應用樣式

        // 顯示附件名稱和附檔名
        const fileName = document.createElement('span');  // 創建一個 <span> 元素
        fileName.textContent = `${file.name}`;  // 將文件名設置為 <span> 元素的文字內容
        
        // 創建一個刪除按鈕，用於移除文件項目
        const removeBtn = document.createElement('button');  // 創建 <button> 元素
        removeBtn.classList.add('remove-btn');  // 為按鈕添加 'remove-btn' class 以便應用樣式
        removeBtn.innerHTML = '&times;';  // 設置按鈕的內容為 '×'，這是刪除的符號
        
        // 當用戶點擊刪除按鈕時，移除相應的文件項目
        removeBtn.addEventListener('click', function() {
            fileList.removeChild(fileItem);  // 將當前的文件項目從文件列表中移除
        });

        // 將文件名稱和刪除按鈕加入 file-item 容器
        fileItem.appendChild(fileName);  // 將文件名 <span> 加入到 file-item 中
        fileItem.appendChild(removeBtn);  // 將刪除按鈕加入到 file-item 中

        // 將 file-item 添加到文件列表容器中，以顯示在網頁上
        fileList.appendChild(fileItem);
    }
});

/* 附件上傳-結束 */
