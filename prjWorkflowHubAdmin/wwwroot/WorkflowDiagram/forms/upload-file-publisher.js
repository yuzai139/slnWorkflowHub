//// 選取相關的 DOM 元素
//const fileInput = document.getElementById('file-input');
//const uploadButton = document.getElementById('upload-button');
//const thumbnailPreview = document.getElementById('thumbnail-preview');
//const dragLabel = document.getElementById('drag-label');
//const fileDragArea = document.getElementById('file-drag-area');

//// 初始化檔案資料
//const fileData = {
//    dataURL: null
//};

//// 點擊按鈕打開文件選擇器
//uploadButton.addEventListener('click', () => {
//    fileInput.click();
//});

//// 當選擇文件時顯示縮圖
//fileInput.addEventListener('change', (event) => {
//    const file = event.target.files[0];
//    if (file) {
//        previewFile(file);
//    }
//});

//// 處理拖曳文件
//if (fileDragArea) {

//fileDragArea.addEventListener('dragover', (event) => {
//    event.preventDefault();
//    event.stopPropagation();
//    dragLabel.style.display = 'none';
//});

//fileDragArea.addEventListener('dragleave', (event) => {
//    event.preventDefault();
//    event.stopPropagation();
//    dragLabel.style.display = 'block';
//});

//fileDragArea.addEventListener('drop', (event) => {
//    event.preventDefault();
//    event.stopPropagation();
//    const file = event.dataTransfer.files[0];
//    if (file) {
//        previewFile(file);
//    }
//});


//}

//// 顯示預覽圖片
//function previewFile(file) {
//    const reader = new FileReader();
//    reader.onload = function (event) {
//        fileData.dataURL = event.target.result;
//        thumbnailPreview.src = fileData.dataURL;
//        thumbnailPreview.style.display = 'block';
//        dragLabel.style.display = 'none';
//    };
//    reader.readAsDataURL(file);
//}
