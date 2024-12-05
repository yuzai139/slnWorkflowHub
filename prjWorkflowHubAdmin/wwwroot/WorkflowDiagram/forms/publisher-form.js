//========【左邊選單】=========
const iconButtonLeft = document.getElementById('icon-button-left');
const formContainerLeft = document.getElementById('form-container-left');
const closeFormButtonLeft = document.getElementById('close-form-button-left');

// 點擊icon按鈕顯示表單
iconButtonLeft.addEventListener('click', function () {
	iconButtonLeft.style.display = 'none'; // 隱藏icon按鈕
	formContainerLeft.style.display = 'flex'; // 顯示表單
});

// 點擊表單右上角icon按鈕隱藏表單
closeFormButtonLeft.addEventListener('click', function () {
	formContainerLeft.style.display = 'none'; // 隱藏表單
	iconButtonLeft.style.display = 'flex'; // 顯示icon按鈕
});
//=======【右側選單】========
const iconButtonRight = document.getElementById('icon-button-right');
const formContainerRight = document.getElementById('form-container-right');
const closeFormButtonRight = document.getElementById('close-form-button-right');

// 點擊icon按鈕顯示表單
iconButtonRight.addEventListener('click', function () {
	iconButtonRight.style.display = 'none'; // 隱藏icon按鈕
	formContainerRight.style.display = 'flex'; // 顯示表單
});

// 點擊表單右上角icon按鈕隱藏表單
closeFormButtonRight.addEventListener('click', function () {
	formContainerRight.style.display = 'none'; // 隱藏表單
	iconButtonRight.style.display = 'flex'; // 顯示icon按鈕
});
//============
// 當上傳按鈕被點擊時，模擬點擊 input[type="file"]
let thumbnailUpload = document.getElementById('thumbnail-upload');

let uploadButton = document.querySelector('upload-button');
if (uploadButton) {
    uploadButton.addEventListener('click', function () {
        thumbnailUpload.click(); // 觸發文件選擇窗口
    });
}

// 切換縮圖 : 監聽文件選擇
if (thumbnailUpload) {
thumbnailUpload.addEventListener('change', function(event) {
    const file = event.target.files[0]; // 獲取上傳的文件
    const previewContainer = document.getElementById('thumbnail-preview'); // 預覽容器
    const reuploadbutton = document.getElementById('reupload-button'); // 重新上傳鏈接

    if (file) {
        const reader = new FileReader();

        // 文件讀取成功後顯示圖片
        reader.onload = function(e) {
            previewContainer.innerHTML = ''; // 清空之前的內容
            const img = document.createElement('img');
            img.src = e.target.result; // 將圖片的 src 設置為上傳的文件
            previewContainer.appendChild(img); // 添加圖片到預覽容器
        }

        reader.readAsDataURL(file); // 開始讀取文件
        reuploadbutton.style.display = 'block'; // 顯示重新上傳鏈接
    }
});

}


// 重新上傳按鈕
let reuploadButton = document.getElementById('reupload-button');
if (reuploadButton) {
    reuploadButton.addEventListener('click', function (event) {
        event.preventDefault();
        document.getElementById('thumbnail-upload').click(); // 重新打開上傳選擇框
    });

}

