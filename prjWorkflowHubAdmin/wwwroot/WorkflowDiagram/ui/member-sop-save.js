const baseAddress = "https://localhost:7146"; // 記得改成伺服器端的實際地址
const viewAddress = "https://localhost:7151";
//觸發的button
const createsopbutton = document.getElementById('createSop');

let newSopdata;

// 在 DOM 加載完成後設定按鈕的 click 事件處理器
document.addEventListener("DOMContentLoaded", function () {
    if (createsopbutton) {
        createsopbutton.addEventListener("click", function () {
            handleCreateSopClick(12);  // 傳入 memberId = 12
        });
    }
    
});

// 獨立的 click 處理函式，負責呼叫 API 並處理回應
async function handleCreateSopClick(memberId) {
    try {
        const response = await fetch(`${baseAddress}/api/TMmbSopCreate/${memberId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error("API 呼叫失敗，狀態碼：" + response.status);
        }

        // 檢查 API 返回的資料
        newSopdata = await response.json();
        console.log("API 回應資料：", newSopdata); // 檢查 API 回應資料的內容

        if (newSopdata && newSopdata.FSopid) {
            console.log("已建立新的SOP資料");

            const redirectUrl = `/SopMember/SopMemberEdit?sopId=${newSopdata.FSopid}`;
            // 使用 window.location.href 進行跳轉
            window.location.href = redirectUrl;
        } else {
            console.log("找不到 SOP 資料。");
        }
    } catch (error) {
        console.error("新增SOP資料時發生錯誤:", error);
    }
}

