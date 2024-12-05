document.addEventListener("DOMContentLoaded", () => {
    const fileInput = document.getElementById("file-upload");
    const fileList = document.getElementById("file-list");
    const saveButton = document.getElementById("save-diagram-button");
    const releaseButton = document.getElementById("release-button"); //儲存並發佈
    const sopId = document.getElementById("sopId").value;
    const maxFileSize = 20 * 1024 * 1024; // 單個文件大小上限 20 MB
    const maxFileCount = 5; // 單次最多上傳文件數量上限 5 個

    let allFiles = []; // 暫存所有選擇的文件，包括已存在的和新上傳的

    // 載入並顯示資料庫中的附件資料
    async function loadAttachments() {
        try {
            const response = await fetch(`/api/TSopAffixes/GetSopFiles/${sopId}`);

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Error ${response.status}: ${errorText}`);
            }

            const data = await response.json();

            // 檢查是否所有項目中的 `FAffixName` 欄位都為空
            if (!data || data.length === 0 || data.every(item => !item.FAffixName)) {
                allFiles = []; // 清空 allFiles
                fileList.innerHTML = ""; // 清空文件列表顯示區域
                console.log("沒有有效附件可顯示");
                return; // 提前結束函數
            }

            // 否則，清空 allFiles 並重新加入來自資料庫的文件
            allFiles = data.map(item => ({
                id: `db-${item.FSopaffixId}`, // 使用資料庫中的 ID 作為識別
                name: item.FAffixName,
                file: null, // 資料庫中的文件不會有 File 物件
                isNew: false,
                downloadPath: item.FAffixPath
            }));

            renderFileList(); // 顯示文件列表
        } catch (error) {
            console.error('Error loading attachments:', error);
            fileList.innerHTML = `<p>載入附件時發生錯誤：${error.message}</p>`;
        }
    }

    // 點擊「選擇文件」按鈕時觸發文件選擇
    document.getElementById("select-files-btn").addEventListener("click", () => {
        fileInput.click();
    });

    fileInput.addEventListener("change", () => {
        // 每次選擇文件時，清空文件列表和暫存文件
        allFiles = [];
        fileList.innerHTML = "";

        const newFiles = Array.from(fileInput.files);

        // 檢查文件數量是否超出上限
        if (newFiles.length > maxFileCount) {
            alert(`每次最多只能上傳 ${maxFileCount} 個附件。`);
            fileInput.value = ""; // 清空選擇
            return;
        }

        // 檢查每個文件大小，並加入 allFiles
        newFiles.forEach(file => {
            if (file.size > maxFileSize) {
                alert(`文件 '${file.name}' 超過了 20 MB 的大小限制，無法上傳。`);
            } else {
                const fileId = `file-${Date.now()}-${Math.random().toString(36).substring(2, 11)}`;
                allFiles.push({
                    id: fileId, // 唯一ID，用於按鈕事件識別
                    name: file.name,
                    file: file,
                    isNew: true
                });
            }
        });

        renderFileList(); // 更新文件列表
        fileInput.value = ""; // 清空輸入，方便再次選擇
    });


    // 渲染文件列表
    function renderFileList() {
        fileList.innerHTML = ""; // 清空列表

        allFiles.forEach((file) => {
            const fileSizeMB = file.file ? (file.file.size / (1024 * 1024)).toFixed(2) : "已存在";
            const fileItem = document.createElement("div");
            fileItem.className = "file-item";
            fileItem.innerHTML = `
                ${file.name} (${fileSizeMB} MB) 
                <button id="${file.id}-download" class="innerhtml-btn" style="cursor:pointer;">下載</button>
            `;
            fileList.appendChild(fileItem);

            // 設定刪除按鈕事件
            //document.getElementById(`${file.id}-remove`).addEventListener("click", () => {
            //    allFiles = allFiles.filter(f => f.id !== file.id);
            //    renderFileList();
            //});

            // 設定下載按鈕事件
            document.getElementById(`${file.id}-download`).addEventListener("click", () => {
                if (file.isNew) {
                    // 新上傳的文件，使用本地URL進行下載
                    const url = URL.createObjectURL(file.file);
                    const downloadLink = document.createElement("a");
                    downloadLink.href = url;
                    downloadLink.download = file.name;
                    downloadLink.click();
                    URL.revokeObjectURL(url);
                } else {
                    // 資料庫中的文件，直接用文件路徑進行下載
                    window.open(file.downloadPath, "_blank");
                }
            });
        });
    }

    // 點擊「儲存」按鈕上傳新文件
    saveButton.addEventListener("click", async () => {
        const formData = new FormData();
        const newFiles = allFiles.filter(file => file.isNew).map(file => file.file);

        if (newFiles.length === 0) {
            console.log("沒有新的文件需要上傳。");
            return;
        }

        // 添加新文件到 FormData
        newFiles.forEach(file => formData.append("files", file));

        try {
            const response = await fetch(`/api/TSopAffixes/UploadSopFile/${sopId}`, {
                method: "PUT",
                body: formData
            });

            if (response.ok) {
                console.log(`成功上傳 ${newFiles.length} 個文件`);
                loadAttachments(); // 重新載入所有文件
            } else {
                const error = await response.json();
                alert(`上傳失敗：${error.message || response.statusText}`);
            }
        } catch (error) {
            console.error("上傳過程中出現錯誤：", error);
            alert("上傳失敗，請稍後再試。");
        }
    });

    // 初始化載入並顯示資料庫中的附件
    loadAttachments();


    if (releaseButton) {

        //儲存並發布
        saveAffix();
    }

    function saveAffix() {

        releaseButton.addEventListener("click", async () => {
            const formData = new FormData();
            const newFiles = allFiles.filter(file => file.isNew).map(file => file.file);

            if (newFiles.length === 0) {
                console.log("沒有新的文件需要上傳。");
                return;
            }

            // 添加新文件到 FormData
            newFiles.forEach(file => formData.append("files", file));

            try {
                const response = await fetch(`/api/TSopAffixes/UploadSopFile/${sopId}`, {
                    method: "PUT",
                    body: formData
                });

                if (response.ok) {
                    console.log(`成功上傳 ${newFiles.length} 個文件`);
                    loadAttachments(); // 重新載入所有文件
                } else {
                    const error = await response.json();
                    alert(`上傳失敗：${error.message || response.statusText}`);
                }
            } catch (error) {
                console.error("上傳過程中出現錯誤：", error);
                alert("上傳失敗，請稍後再試。");
            }
        });

    }


});




