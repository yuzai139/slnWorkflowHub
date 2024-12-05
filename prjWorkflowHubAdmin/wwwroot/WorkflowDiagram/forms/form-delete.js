const baseAddress = 'https://localhost:7151'; //在core view 專案
document.addEventListener("DOMContentLoaded", function () {

    const deletesop = document.getElementById("deleteSop");
    const deletePublishersop = document.getElementById("deletePublisherSop");

    if (deletesop) {

        deletesop.addEventListener('click', function (event) {


            // 彈出確認框，只有當用戶點擊「確定」時才繼續
            if (confirm('確定要刪除嗎？')) {
                // 取得 sopId 的值
                let sopID = document.getElementById("sopId").value;

                if (sopID) {
                    fetch(`${baseAddress}/api/SopMemberApi/${sopID}`, {
                        method: 'DELETE'
                    })
                        .then(response => {
                            if (!response.ok) {
                                throw new Error(`刪除失敗，狀態碼：${response.status}`);
                            }
                            return response.json();
                        })
                        .then(data => {
                            console.log(data.message); // 刪除成功訊息

                            // 跳轉回列表頁
                            const redirectUrl = `http://localhost:4200/workflow/memberworkflow-index`;
                            window.location.href = redirectUrl;
                        })
                        .catch(error => console.error('Error:', error));
                } else {
                    console.log("sopId 沒有值");
                }
            }

        });

    }


    if (deletePublishersop) {

        deletePublishersop.addEventListener('click', function (event) {


            // 彈出確認框，只有當用戶點擊「確定」時才繼續
            if (confirm('確定要刪除嗎？')) {
                // 取得 sopId 的值
                let sopID = document.getElementById("sopId").value;

                if (sopID) {
                    fetch(`${baseAddress}/api/SopMemberApi/${sopID}`, {
                        method: 'DELETE'
                    })
                        .then(response => {
                            if (!response.ok) {
                                throw new Error(`刪除失敗，狀態碼：${response.status}`);
                            }
                            return response.json();
                        })
                        .then(data => {
                            console.log(data.message); // 刪除成功訊息

                            // 跳轉回列表頁
                            const redirectUrl = `http://localhost:4200/workflow/publishermanage-workflowlist`;
                            window.location.href = redirectUrl;
                        })
                        .catch(error => console.error('Error:', error));
                } else {
                    console.log("sopId 沒有值");
                }
            }

        });

    }
    

});//DOMContentLoaded 結尾
