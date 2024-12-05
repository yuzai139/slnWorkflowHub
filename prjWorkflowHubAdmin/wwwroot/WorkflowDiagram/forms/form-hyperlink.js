document.addEventListener("DOMContentLoaded", function () {

    const backToHome = document.getElementById("backTohome");
    const backToIndex = document.getElementById("backToIndex");
    const backToPubSopList = document.getElementById("backToPubSopList");


    const redirectUrl = `http://localhost:4200/workflow/memberworkflow-index`;
    const redirectPubListUrl = `http://localhost:4200/workflow/publishermanage-workflowlist`;
    const homeUrl = `http://localhost:4200/common/home`;


    backToHome.addEventListener('click', function (event) {

        /*event.preventDefault();  // 阻止預設跳轉行為*/

        //if (confirm("確定要返回首頁嗎?")) {

        // 使用 window.location.href 進行跳轉
        window.location.href = homeUrl;

        //}

    });



    if (backToIndex != null) {

        backToIndex.addEventListener('click', function (event) {

            /*event.preventDefault();  // 阻止預設跳轉行為*/

            if (confirm("確定要離開嗎？未儲存的項目會遺失")) {

                // 使用 window.location.href 進行跳轉
                window.location.href = redirectUrl;

            }

        });

    }


    if (backToPubSopList != null) {

        backToPubSopList.addEventListener('click', function (event) {

            /*event.preventDefault();  // 阻止預設跳轉行為*/

            if (confirm("確定要離開嗎？未儲存的項目會遺失")) {

                // 使用 window.location.href 進行跳轉
                window.location.href = redirectPubListUrl;

            }

        });

    }
    


    


});//DOMContentLoaded 結尾