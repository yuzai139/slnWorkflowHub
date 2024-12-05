function closeAlert() {
    var alertBox = document.getElementById('alert-box2');
    if (alertBox) {
        alertBox.classList.add('hide');
        setTimeout(function () {
            alertBox.style.display = 'none';
        }, 500);
    }
}

// 將函數綁定到 window，這樣可以在全局作用域中使用
window.closeAlert = closeAlert;

// 自動關閉 alert，10 秒後觸發
setTimeout(closeAlert, 10000);
