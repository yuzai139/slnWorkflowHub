// 引入各種功能模組
import { canvasClear } from '../diagram/canvas-clear.js'; // 清除畫布
import { dgrmPngChunkGet, dgrmPngCreate } from '../diagram/dgrm-png.js'; // 處理 PNG 圖片的功能
import { deserialize, serialize } from '../diagram/dgrm-serialization.js'; // 序列化和反序列化圖表
import { generateKey, srvSave } from '../diagram/dgrm-srv.js'; // 生成密鑰並保存到服務端
import { fileOpen, fileSave } from '../infrastructure/file.js'; // 文件打開和保存功能
import { tipShow, uiDisable } from './ui.js'; // 提示顯示和 UI 禁用功能

// 定義菜單類別，繼承自 HTMLElement
//const fsopname = document.querySelector('input[name="MemberSopViewModel.FSopName"]').value;
//const sopname = document.getElementById('sopName');

export class Menu extends HTMLElement {
	connectedCallback() {
		const shadow = this.attachShadow({ mode: 'closed' });// 使用 Shadow DOM 隱藏內部結構
		shadow.innerHTML = `
			<style>
			.menu {
				position: fixed;
				top: 15px;
				left: 15px;
				cursor: pointer;
				z-index: 11; /* 原本沒有z-index */
				display: none; /* 元素不可見，且不佔據空間 */
			}
			#options {
				position: fixed;
				padding: 15px;
				box-shadow: 0px 0px 58px 2px rgb(34 60 80 / 20%);
				border-radius: 16px;
				background-color: rgba(255,255,255, .9);

				top: 0px;
				left: 0px;

				z-index: 11; /* 原本z-index是1 */
			}

			#options div, #options a { 
				color: rgb(13, 110, 253); 
				cursor: pointer; margin: 10px 0;
				display: flex;
				align-items: center;
				line-height: 25px;
				text-decoration: none;
			}
			#options div svg, #options a svg { margin-right: 10px; }
			</style>
			<svg id="menu" class="menu" viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3V4zm0 7h18v2H3v-2zm0 7h18v2H3v-2z" fill="rgb(52,71,103)"/></svg>
			<div id="options" style="visibility: hidden;">
			 	<div id="menu2" style="margin: 0 0 15px;"><svg viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3V4zm0 7h18v2H3v-2zm0 7h18v2H3v-2z" fill="rgb(52,71,103)"/></svg></div>
				<div id="new"><svg viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M9 2.003V2h10.998C20.55 2 21 2.455 21 2.992v18.016a.993.993 0 0 1-.993.992H3.993A1 1 0 0 1 3 20.993V8l6-5.997zM5.83 8H9V4.83L5.83 8zM11 4v5a1 1 0 0 1-1 1H5v10h14V4h-8z" fill="rgb(52,71,103)"/></svg>New diagram</div>
				<div id="open"><svg viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 21a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1h7.414l2 2H20a1 1 0 0 1 1 1v3h-2V7h-7.414l-2-2H4v11.998L5.5 11h17l-2.31 9.243a1 1 0 0 1-.97.757H3zm16.938-8H7.062l-1.5 6h12.876l1.5-6z" fill="rgb(52,71,103)"/></svg>Open diagram image</div>
				<div id="save"><svg viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 19h18v2H3v-2zm10-5.828L19.071 7.1l1.414 1.414L12 17 3.515 8.515 4.929 7.1 11 13.17V2h2v11.172z" fill="rgb(52,71,103)"/></svg>儲存成圖檔</div>
				<div id="link"><svg viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M13.06 8.11l1.415 1.415a7 7 0 0 1 0 9.9l-.354.353a7 7 0 0 1-9.9-9.9l1.415 1.415a5 5 0 1 0 7.071 7.071l.354-.354a5 5 0 0 0 0-7.07l-1.415-1.415 1.415-1.414zm6.718 6.011l-1.414-1.414a5 5 0 1 0-7.071-7.071l-.354.354a5 5 0 0 0 0 7.07l1.415 1.415-1.415 1.414-1.414-1.414a7 7 0 0 1 0-9.9l.354-.353a7 7 0 0 1 9.9 9.9z" fill="rgb(52,71,103)"/></svg>Copy link to diagram</div>
		 	</div>`;

		const options = shadow.getElementById('options');// 選項面板
		function toggle() {
			 options.style.visibility = options.style.visibility === 'visible' ? 'hidden' : 'visible'; 
			}

		/** @param {string} id, @param {()=>void} handler */
		function click(id, handler) {
			shadow.getElementById(id).onclick = _ => {
				uiDisable(true);
				handler();
				toggle();
				uiDisable(false);
			};
		}

		shadow.getElementById('menu').onclick = toggle;// 點擊菜單圖標切換選項顯示
		shadow.getElementById('menu2').onclick = toggle;

		// 新建圖表的處理
        click('new', () => { 
            canvasClear(this._canvas); 
            tipShow(true); 
        });

        // 保存圖表為 PNG 圖片的處理
        click('save', () => {
            const serialized = serialize(this._canvas); // 序列化圖表數據
            if (serialized.s.length === 0) { 
                alertEmpty(); 
                return; 
            }

            dgrmPngCreate(
                this._canvas,
                JSON.stringify(serialized),
                png => fileSave(png, 'dgrm.png')
            );
        });

		// 打開 PNG 圖片並載入圖表數據
        click('open', () =>
            fileOpen('.png', async png => await loadData(this._canvas, png))
        );

        // 生成圖表連結並複製到剪貼板
        click('link', async () => {
            const serialized = serialize(this._canvas);
            if (serialized.s.length === 0) { 
                alertEmpty(); 
                return; 
            }

			const key = generateKey();
			const url = new URL(window.location.href);
			url.searchParams.set('k', key);
			// use clipboard before server call - to fix 'Document is not focused'
			await navigator.clipboard.writeText(url.toString());
			await srvSave(key, serialized);

			alert('Link to diagram copied to clipboard');
		});
	}

	/** @param {CanvasElement} canvas */
    init(canvas) {
        /** @private */ this._canvas = canvas;

        // 處理文件拖放事件
        document.body.addEventListener('dragover', evt => { 
            evt.preventDefault(); 
        });
        document.body.addEventListener('drop', async evt => {
            evt.preventDefault();

            if (evt.dataTransfer?.items?.length !== 1 ||
                evt.dataTransfer.items[0].kind !== 'file' ||
                evt.dataTransfer.items[0].type !== 'image/png') {
                alertCantOpen(); 
                return;
            }

            await loadData(this._canvas, evt.dataTransfer.items[0].getAsFile());
        });
    }
};
customElements.define('ap-menu', Menu);// 註冊自定義元素 'ap-menu'

/** @param {CanvasElement} canvas,  @param {Blob} png  */
async function loadData(canvas, png) {
    const dgrmChunk = await dgrmPngChunkGet(png); // 從 PNG 中獲取圖表數據
    if (!dgrmChunk) { 
        alertCantOpen(); 
        return; 
    }
    if (deserialize(canvas, JSON.parse(dgrmChunk))) { 
        tipShow(false); 
    }
}

const alertCantOpen = () => alert('無法讀取檔案，請使用你從網站下載的png圖檔');
const alertEmpty = () => alert('畫布是空的');

/** @typedef { {x:number, y:number} } Point */
/** @typedef { import('../infrastructure/canvas-smbl.js').CanvasElement } CanvasElement */

//======wwwwwwwwwwwwwwwwww========= 基本的 API 位址，保持不變 ====wwwwwwwwwwwwwwww==========
var baseAddress = "https://localhost:7146"; // 記得改成伺服器端的實際地址

/* =====把圖表下載'save'加入到新UI====== */
document.getElementById('download-button').addEventListener('click', () => {
    const canvas = document.querySelector('#canvas');

    // 確保有圖表可以儲存
    const serialized = serialize(canvas);
    if (serialized.s.length === 0) {
        alert("流程圖為空");
        return;
    }

    // 呼叫現有的儲存邏輯
    dgrmPngCreate(
        canvas,
        JSON.stringify(serialized),
        png => fileSave(png, '流程圖.png')
    );
});





/* ======重置畫布==================== */
// 監聽「重置畫布」按鈕的點擊事件
document.getElementById('reset-canvas').addEventListener('click', function() {
    // 假設 canvas 是你要清除的畫布元素
    const canvas = document.querySelector('#canvas'); // 替換成實際的畫布 ID
    canvasClear(canvas);  // 清除畫布內容
    tipShow(true);  // 顯示提示，表示畫布已經被重置或新建
});

/* ========打開 PNG 文件======== */
// 監聽「測試-把電腦中圖片載入畫布」按鈕的點擊事件
document.getElementById('load-image-button').addEventListener('click', function() {
    // 調用 fileOpen 函數來打開 PNG 文件
    fileOpen('.png', async (png) => {
        const canvas = document.querySelector('#canvas'); // 替換為你的畫布元素 ID
        await loadData(canvas, png);  // 將 PNG 圖片載入到畫布中
    });
});




// ========================將圖片加載到 Canvas 的函數====================
async function loadDiagramImage(imagePath) {
    const fullImageUrl = `${window.location.origin}/Workflow/SopImages/${imagePath}`;
    try {
        const response = await fetch(fullImageUrl);
        if (!response.ok) {
            throw new Error("無法加載圖片，請檢查圖片路徑或伺服器響應。");
        }

        const blob = await response.blob(); // 將響應轉換為 Blob 文件
        const file = new File([blob], imagePath, { type: 'image/png' }); // 創建一個 File 對象
        const canvas = document.querySelector('#canvas'); // 獲取 canvas 元素

        // 調用 loadData 方法將圖片載入到畫布
        await loadData(canvas, file);
    } catch (error) {
        console.error("圖片加載失敗：", error);
        alert("無法加載圖片，請檢查圖片路徑或伺服器響應。");
    }
}






//============儲存圖檔和資料到後端===================
document.getElementById('save-diagram-button').addEventListener('click', async function () {
    const SopId = document.getElementById('sopId').value
    const sopType = document.getElementById('sopType').value
    const canvas = document.querySelector('#canvas');  // 確保獲取畫布
    const serialized = serialize(canvas); // 序列化圖表數據

    // 取得輸入框的值
    const sopNameInput = document.getElementById('inputSopName').value;
    // 取得顯示框的值
    const sopNameDisplay = document.getElementById('sopName');
    // 將輸入框的值更新到 <p> 元素中
    sopNameDisplay.innerText = sopNameInput;

    //目前時間
    const currentTime = new Date().toLocaleString('zh-TW', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: true
    });


   if (serialized.s.length !== 0) {
        // 使用 dgrmPngCreate 將畫布轉換為 PNG Blob
        dgrmPngCreate(
            canvas,
            JSON.stringify(serialized),
            async (pngBlob) => {  // 將畫布內容轉換為 PNG Blob
                // 創建 FormData，並將 PNG Blob 添加到表單數據中
                const formData = new FormData();
                formData.append('diagramPng', pngBlob, 'diagram.png'); // 在 FormData 中添加一個名為 'diagramPng' 的 PNG 文件

                // 從隱藏欄位中取得 sopId
                console.log('sopId:', SopId);  // 確認 sopId 是否正確
                formData.append('sopId', SopId); // 將 sopId 一起發送到後端

                // 發送 POST 請求到後端，將圖檔數據發送到 '/WorkflowMember/SaveDiagram' URL 進行存儲
                const response = await fetch('/WorkflowMember/SaveDiagram', {
                    method: 'POST',  // 使用 POST 方法
                    body: formData   // 將 formData 作為請求的 body 發送
                });

                console.log(response)

                // 獲取後端回應的結果，並將其轉換為 JSON
                const result = await response.json();

                // 檢查儲存圖檔是否成功
                if (result.success) {
                    console.log('流程圖已成功儲存，路徑:', result.path); // 如果儲存成功，顯示成功訊息

                    if (sopType == 1) {
                        let editTimeElement = document.getElementById('sopEditTime');
                        editTimeElement.innerText = currentTime; //如果是membersop, 存editTime
                        // 繼續儲存TSop資料
                        await saveSopData(SopId, result.path, currentTime); 
                    }

                    if (sopType == 2) {
                        
                        //更新發佈者圖檔
                        await uploadImage(SopId);

                        // 繼續儲存TSop資料
                        await savePubSop(SopId);
                    }

                } else {
                    alert('流程圖儲存失敗');  // 如果儲存失敗，顯示錯誤訊息
                }
            }
        );
    } else {
       // 如果流程圖是空的，依然可以儲存其他TSop資料
       if (sopType == 1) {
           //會員資料
           let editTimeElement = document.getElementById('sopEditTime');
           editTimeElement.innerText = currentTime; //如果是membersop, 存editTime
           await saveSopData(SopId, null, currentTime); // 如果沒有流程圖，path 傳 null
       }

       if (sopType == 2) {

           
            //更新發佈者圖檔
            await uploadImage(SopId);
           
           // 繼續儲存TSop資料
           await savePubSop(SopId);
       }

    }

});

//============儲存TSop資料的函數 (MemberSop)  ==========
async function saveSopData(sopId, imagePath, currentTime) {

    const SopId = document.getElementById("sopId").value;
    const memberId = document.getElementById("sopMemberId").value;
    const memberSopName = document.getElementById("inputSopName").value;
    const memberSopDescription = document.getElementById("sopDescription").value;
    const diagramImagePath = imagePath;
    const memberSopType = document.getElementById("sopType").value;
    const memberSopFileStatus = document.getElementById("sopFileStatus").value;
    const memberSharePermission = document.getElementById("share-permission").value;
    const memberSopDepartment = document.getElementById("sopDepartment").value;
    const memberSopCustomer = document.getElementById("sopCustomer").value;
    const memberSopBusiness = document.getElementById("sopBusiness").value;
    const hiddenJobItemId = document.getElementById("hiddenJobItem").value;
    const hiddenIndustryId = document.getElementById("hiddenIndustry").value;
    const companySizeSelect = document.getElementById("companySizeSelect").value;
    const editTime = currentTime;

    let sopData = {

        FSopid: Number(sopId), 
        FMemberId: Number(memberId),
        FSopName: memberSopName,
        FSopDescription: memberSopDescription,
        FSopFlowImagePath: diagramImagePath,
        FSopType: Number(memberSopType),
        FFileStatus: memberSopFileStatus,
        FCompanySize: companySizeSelect,
        FSharePermission: memberSharePermission,
        FDepartment: memberSopDepartment,
        FCustomer: memberSopCustomer,
        FBusiness: memberSopBusiness,
        FJobItemId: Number(hiddenJobItemId),
        FIndustryId: Number(hiddenIndustryId),
        FEditTime:editTime, 
    };

    console.log('儲存的資料:',sopData);

    try {
        const response = await fetch(`${baseAddress}/api/TMmbSops/${sopId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(sopData),
        });

        if (response.ok) {
            alert('資料已成功儲存');
        } else {
            //const errorData = await response.json();
            //alert('儲存資料失敗：' + errorData.message);
            const errorText = await response.text();
            console.error('儲存失敗：', errorText);
        }
    } catch (error) {
        console.error('儲存 TSop 資料過程中出現錯誤：', error);
        alert('發生錯誤，請稍後重試。');
    }
};



//============ 儲存TSop資料的函數 (PublisherSop) ==========
async function savePubSop(sopId) {

    const memberId = document.getElementById("sopMemberId").value;
    const sopType = document.getElementById("sopType").value;
    const publisherId = document.getElementById("publisherId").value;
    const sopName = document.getElementById("inputSopName").value;
    const sopDescription = document.getElementById("sopDescription").value;
    const pubContent = document.getElementById("pubContent").value;
    const sopReleaseStatus = document.getElementById("releaseStatus").value;
    const isRelease = document.getElementById("isRelease").value;
    const sopDepartment = document.getElementById("sopDepartment").value;
    const sopCustomer = document.getElementById("sopCustomer").value;
    const sopBusiness = document.getElementById("sopBusiness").value;
    const sopPrice = document.getElementById("sopPrice").value;
    const sopSalePoints = document.getElementById("salePoints").value;
    const hiddenJobItemId = document.getElementById("hiddenJobItem").value;
    const hiddenIndustryId = document.getElementById("hiddenIndustry").value;
    const companySizeSelect = document.getElementById("companySizeSelect").value;
    const releaseTime = document.getElementById("sopReleaseTime").innerText;


    let sopData = {

        FSopid: Number(sopId),
        FMemberId: Number(memberId),
        FSopType: Number(sopType),
        FPublisherId: Number(publisherId),
        FSopName: sopName,
        FSopDescription: sopDescription,
        FPubContent: pubContent,
        FReleaseStatus: sopReleaseStatus,
        FIsRelease: Boolean(isRelease === "true" || isRelease === "1"), // 使用 Boolean() 正確轉換布林值
        FCompanySize: companySizeSelect,
        FDepartment: sopDepartment,
        FCustomer: sopCustomer,
        FBusiness: sopBusiness,
        FJobItemId: Number(hiddenJobItemId),
        FIndustryId: Number(hiddenIndustryId),
        FPrice: parseFloat(sopPrice),   // 使用 parseFloat 轉換為浮點數
        FSalePoints: parseFloat(sopSalePoints), // 使用 parseFloat 轉換為浮點數
        FReleaseTime: releaseTime,
    };

    console.log('儲存的資料:', sopData);

    try {
        const response = await fetch(`${baseAddress}/api/TPublisherSop/${sopId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(sopData),
        });

        if (response.ok) {
            alert('資料已成功儲存');
        } else {
            const errorText = await response.text();
            console.error('儲存失敗：', errorText);
        }
    } catch (error) {
        console.error('儲存 TSop 資料過程中出現錯誤：', error);
        alert('發生錯誤，請稍後重試。');
    }
};







//=========================   綁定畫面元素 + 載入畫面資料  =========================
document.addEventListener("DOMContentLoaded", function () {
    
         //member 和 publisher 通用的欄位
        const SopId = document.getElementById("sopId").value; //sopId //這裡已經知道sopId是多少
        const sopTypeElement = document.getElementById("sopType");
        const sopMemberIdElement = document.getElementById("sopMemberId");
        const diagramImagePath = document.getElementById("diagramImagePath");
        const sopNameDisplay = document.getElementById("sopName");
        const sopNameInput = document.getElementById("inputSopName");
        const hiddenJobItemId = document.getElementById("hiddenJobItem");
        const hiddenIndustryId = document.getElementById("hiddenIndustry");
        const jobNameDisplay = document.getElementById("jobName");
        const industryNameDisplay = document.getElementById("industryName");
        const sopDepartmentInput = document.getElementById("sopDepartment");
        const sopCustomerInput = document.getElementById("sopCustomer");
        const sopBusinessInput = document.getElementById("sopBusiness");
        const sopDescriptionTextarea = document.getElementById("sopDescription");
        const companySizeSelect = document.getElementById("companySizeSelect");

        const baseAddress = "https://localhost:7146"; //serverAddress

        // 調用 API 並將數據更新到頁面
        fetch(`${baseAddress}/api/TMmbSopCreate/${SopId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("API 請求失敗，狀態碼：" + response.status);
                }
                return response.json();
            })
            .then(data => {

                if (data.FSopType == 1) {
                    console.log("這是會員SOP");
                    console.log("GET 會員SOP的API回應資料：", data);
                    // 綁定 HTML 元素
                    const sharePermissionSelect = document.getElementById("share-permission");
                    const sopFileStatusSelect = document.getElementById("sopFileStatus");
                    const sopEditTimeDisplay = document.getElementById("sopEditTime");

                    // 更新隱藏欄位的值
                    sopTypeElement.value = data.FSopType || '';
                    sopMemberIdElement.value = data.FMemberId || '';
                    hiddenJobItemId.value = data.FJobItemId || '';
                    hiddenIndustryId.value = data.FIndustryId || '';

                    // 更新顯示的元素值
                    sopNameDisplay.textContent = data.FSopName || '';
                    sopNameInput.value = data.FSopName || '';
                    jobNameDisplay.textContent = data.JobItem || '';
                    industryNameDisplay.textContent = data.Industry || '';

                    // 其他資料欄位的更新
                    sopDepartmentInput.value = data.FDepartment || '';
                    sopCustomerInput.value = data.FCustomer || '';
                    sopBusinessInput.value = data.FBusiness || '';
                    sharePermissionSelect.value = data.FSharePermission || '';
                    sopFileStatusSelect.value = data.FFileStatus || '';
                    sopEditTimeDisplay.textContent = data.FEditTime || '';
                    sopDescriptionTextarea.value = data.FSopDescription || '';

                    // 設置公司規模下拉選單的值
                    companySizeSelect.value = data.FCompanySize || '';

                    //圖片路徑
                    diagramImagePath.value = data.FSopFlowImagePath;

                    ////公司規模的API打完之後，再傳一次到畫面上
                    //let a = document.getElementById("hiddenCompanySize").value;
                    //companySizeSelect.value = a;

                    

                }
                if (data.FSopType == 2) {

                    // 綁定Publisher獨有的 HTML 元素
                    const publisherId = document.getElementById("publisherId");
                    const pubContent = document.getElementById("pubContent");
                    const pubImage = document.getElementById("publishImagePath");
                    const sopReleaseStatus = document.getElementById("releaseStatus");
                    const isRelease = document.getElementById("isRelease");
                    const sopPrice = document.getElementById("sopPrice");
                    const sopSalePoints = document.getElementById("salePoints");
                    const releaseTime = document.getElementById("sopReleaseTime");

                    //更新Publisher獨有的欄位
                    releaseTime.textContent = data.FReleaseTime || '';
                    sopReleaseStatus.value = data.FReleaseStatus || '';
                    publisherId.value = data.FPublisherId || '';
                    pubContent.value = data.FPubContent || '';
                    pubImage.value = data.FPubSopImagePath ; //發佈者sop圖片
                    isRelease.value = data.FIsRelease || '';
                    sopPrice.value = data.FPrice || '';
                    sopSalePoints.value = data.FSalePoints || '';


                    // 更新隱藏欄位的值
                    sopTypeElement.value = data.FSopType || '';
                    sopMemberIdElement.value = data.FMemberId || '';
                    hiddenJobItemId.value = data.FJobItemId || '';
                    hiddenIndustryId.value = data.FIndustryId || '';

                    // 更新顯示的元素值
                    sopNameDisplay.textContent = data.FSopName || '';
                    sopNameInput.value = data.FSopName || '';
                    jobNameDisplay.textContent = data.JobItem || '';
                    industryNameDisplay.textContent = data.Industry || '';

                    // 其他資料欄位的更新
                    sopDepartmentInput.value = data.FDepartment || '';
                    sopCustomerInput.value = data.FCustomer || '';
                    sopBusinessInput.value = data.FBusiness || '';
                    sopDescriptionTextarea.value = data.FSopDescription || '';

                    // 設置公司規模下拉選單的值
                    companySizeSelect.value = data.FCompanySize || '';

                    //圖片路徑
                    diagramImagePath.value = data.FSopFlowImagePath;

                    //因為順序關係，發佈按鈕的載入要放在這裡(發佈者sop)
                    releaseButton();
                    //下架按鈕
                    removefromMarket();


                    //加載發佈者sop圖片(只有發佈者有，所以寫在裡面)
                    if (pubImage.value && pubImage.value != '') {  // 檢查圖片路徑是否存在
                        console.log(pubImage.value);
                        const thumbnailPreview = document.getElementById("thumbnail-preview");
                        thumbnailPreview.src = `/Workflow/PublishImages/${pubImage.value}`; // 設定圖片預覽的路徑
                        thumbnailPreview.style.display = "block"; // 顯示圖片
                    }

                    if (sopReleaseStatus.value == "已發佈") {
                        document.getElementById("release-button").style.display = "none"; // 隱藏發佈按鈕
                        document.getElementById("removed-from-market").style.display = "block"; // 顯示下架按鈕
                    }
                    else if (sopReleaseStatus.value == "未發佈") {
                        document.getElementById("release-button").style.display = "block"; // 隱藏發佈按鈕
                        document.getElementById("removed-from-market").style.display = "none"; // 顯示下架按鈕
                    }
                }

                

                // 加載畫布圖片（若存在）(會員跟發佈者都有，所以寫在外面)
                if (diagramImagePath.value) {
                    loadDiagramImage(diagramImagePath.value);
                } else {
                    console.log("沒有找到可加載的流程圖。");
                }
 
            })
            .catch(error => {
                console.error("載入 SOP 資料時發生錯誤:", error);
            });


    // =============================  選擇發佈者圖片  ========================================
    // 點擊「選擇圖片」按鈕模擬 file input 點擊
    document.getElementById("select-image-button").addEventListener("click", () => {
        document.getElementById("file-input").click();
    });

    // 即時預覽圖片
    document.getElementById("file-input").addEventListener("change", () => {
        const fileInput = document.getElementById("file-input");
        const file = fileInput.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                document.getElementById("thumbnail-preview").src = e.target.result;
                document.getElementById("thumbnail-preview").style.display = "block";
            };
            reader.readAsDataURL(file); // 將圖片轉為 base64 URL 進行預覽
        }
    });

});


// ===== 增加會員點數的方法  =====
async function addMemberPoints(memberId, addPoint) {
    try {
        // 定義 API 的 URL，使用模板字串來插入 `memberId` 和 `addPoint`
        const url = `${baseAddress}/api/TMemberPoint/add/${memberId}/${addPoint}`;

        // 發送 PUT 請求
        const response = await fetch(url, {
            method: 'PUT', // 使用 PUT 方法
            headers: {
                'Content-Type': 'application/json' // 設置內容類型
            }
        });

        // 處理回應
        if (!response.ok) {
            // 錯誤處理
            const errorMessage = await response.text();
            console.error("等待回應時發生錯誤：", errorMessage);
            return;
        }

        // 成功回應
        const result = await response.json();
        console.log("點數增加成功：", result);
    } catch (error) {
        // 錯誤處理
        console.error("新增點數時發生錯誤：", error);
    }
}


// =========================  下架工作流程  =============================

var removefromMarket = function removeFromMarket() {

document.getElementById('removed-from-market').addEventListener('click', async function () {
    const sopId = document.getElementById('sopId').value;

    // 更新資料庫中的 releaseStatus 狀態
    const sopReleaseStatus = "未發佈";
    const sopData = {
        FSopid: Number(sopId),
        FReleaseStatus: sopReleaseStatus,
        FReleaseTime: '-',
    };

    console.log("下架sop",sopData);

    try {
        const response = await fetch(`${baseAddress}/api/TPublisherSop/PutReleaseStatus/${sopId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(sopData),
        });

        if (response.ok) {
            alert('工作流程已成功下架');
            document.getElementById("releaseStatus").value = sopReleaseStatus; // 更新前端的 releaseStatus
            document.getElementById("sopReleaseTime").textContent = sopData.FReleaseTime;
            document.getElementById("removed-from-market").style.display = "none"; // 隱藏下架按鈕
            document.getElementById("release-button").style.display = "block"; // 顯示發佈按鈕
        } else {
            const errorText = await response.text();
            console.error('下架失敗：', errorText);
            alert("下架失敗，請稍後重試");
        }
    } catch (error) {
        console.error('下架過程中出現錯誤：', error);
        alert('下架失敗，請稍後重試');
    }
});

}



//==========   更新發佈者圖片    ===========
// 上傳圖片函數
async function uploadImage(sopId) {
    const fileInput = document.getElementById("file-input");
    if (fileInput.files.length === 0) {
        /*alert("請選擇圖片");*/
        console.log("未選擇工作流程商品圖")
        return;
    }

    const file = fileInput.files[0];
    const formData = new FormData();
    formData.append("file", file);

    try {
        const response = await fetch(`/api/SopPublisherApi/UploadImage/${sopId}`, {
            method: "PUT",
            body: formData,
        });

        if (response.ok) {
            const result = await response.json();
            console.log(result.message);
            document.getElementById("publishImagePath").value = result.ImagePath; // 更新隱藏欄位
        } else {
            console.error("工作流程商品圖片上傳失敗", await response.text());
            alert("工作流程商品圖片上傳失敗，請重試");
        }
    } catch (error) {
        console.error("上傳過程發生錯誤：", error);
        alert("上傳失敗，請稍後再試");
    }
}





//===============  把發佈按鈕包成一個方法  ================
var releaseButton = function release() {


        //=============================== 儲存&發佈 Sop ========================================
        document.getElementById('release-button').addEventListener('click', async function () {
            const SopId = document.getElementById('sopId').value
            const sopType = document.getElementById('sopType').value
            const canvas = document.querySelector('#canvas');  // 確保獲取畫布
            const serialized = serialize(canvas); // 序列化圖表數據

            // 取得輸入框的值
            const sopNameInput = document.getElementById('inputSopName').value;
            // 取得顯示框的值
            let sopNameDisplay = document.getElementById('sopName');
            // 將輸入框的值更新到 <p> 元素中
            sopNameDisplay.innerText = sopNameInput;

            //目前時間
            const currentTime = new Date().toLocaleString('zh-TW', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: true
            });


            if (serialized.s.length !== 0) {
                // 使用 dgrmPngCreate 將畫布轉換為 PNG Blob
                dgrmPngCreate(
                    canvas,
                    JSON.stringify(serialized),
                    async (pngBlob) => {  // 將畫布內容轉換為 PNG Blob
                        // 創建 FormData，並將 PNG Blob 添加到表單數據中
                        const formData = new FormData();
                        formData.append('diagramPng', pngBlob, 'diagram.png'); // 在 FormData 中添加一個名為 'diagramPng' 的 PNG 文件

                        // 從隱藏欄位中取得 sopId
                        console.log('sopId:', SopId);  // 確認 sopId 是否正確
                        formData.append('sopId', SopId); // 將 sopId 一起發送到後端

                        // 發送 POST 請求到後端，將圖檔數據發送到 '/WorkflowMember/SaveDiagram' URL 進行存儲
                        const response = await fetch('/WorkflowMember/SaveDiagram', {
                            method: 'POST',  // 使用 POST 方法
                            body: formData   // 將 formData 作為請求的 body 發送
                        });

                        console.log(response)

                        // 獲取後端回應的結果，並將其轉換為 JSON
                        const result = await response.json();

                        // 檢查儲存圖檔是否成功
                        if (result.success) {
                            console.log('流程圖已成功儲存，路徑:', result.path); // 如果儲存成功，顯示成功訊息

                            if (sopType == 2) {
                                //更新畫面上的發佈時間
                                let releaseTimeDisplay = document.getElementById("sopReleaseTime");
                                releaseTimeDisplay.innerText = currentTime;
                                //更新發佈者圖檔
                                await uploadImage(SopId);
                                // 繼續儲存TSop資料
                                await releasePubSopdata(SopId, currentTime);
                            }
                        } else {
                            alert('流程圖儲存失敗');  // 如果儲存失敗，顯示錯誤訊息
                        }
                    }
                );
            } else {
                // 如果流程圖是空的，依然可以儲存其他TSop資料
                if (sopType == 2) {
                    //更新畫面上的發佈時間
                    let releaseTimeDisplay = document.getElementById("sopReleaseTime");
                    releaseTimeDisplay.innerText = currentTime;
                    //更新發佈者圖檔
                    await uploadImage(SopId);
                    // 如果沒有流程圖，path 傳 null
                    await releasePubSopdata(SopId, currentTime); 
                }

            }

        });



        //============  儲存&發佈TSop資料的函數 (PublisherSop)  ==========
        async function releasePubSopdata(sopId, currentTime) {

            const memberId = document.getElementById("sopMemberId").value;
            const sopType = document.getElementById("sopType").value;
            const publisherId = document.getElementById("publisherId").value;
            const sopName = document.getElementById("inputSopName").value;
            const sopDescription = document.getElementById("sopDescription").value;
            const pubContent = document.getElementById("pubContent").value;
            let sopReleaseStatus = document.getElementById("releaseStatus").value;
            /*const pubImage = document.getElementById("publishImagePath").value;*/
            let isRelease = document.getElementById("isRelease").value;
            const sopDepartment = document.getElementById("sopDepartment").value;
            const sopCustomer = document.getElementById("sopCustomer").value;
            const sopBusiness = document.getElementById("sopBusiness").value;
            const sopPrice = document.getElementById("sopPrice").value;
            const sopSalePoints = document.getElementById("salePoints").value;
            const hiddenJobItemId = document.getElementById("hiddenJobItem").value;
            const hiddenIndustryId = document.getElementById("hiddenIndustry").value;
            const companySizeSelect = document.getElementById("companySizeSelect").value;
            let releaseTime = document.getElementById("sopReleaseTime").value;
            releaseTime = currentTime;

            if (isRelease === 0 || isRelease === "false" || isRelease === null || isRelease === "") {
                isRelease = "true"; //將isRelease設為true (發佈過)
                const addPoint = 300;
                addMemberPoints(memberId, addPoint);
                alert("此工作流程首次發佈，獲得點數300點!");
            }

            if (sopReleaseStatus == "未發佈") {
                sopReleaseStatus = "已發佈";
                console.log("從未發佈轉換為已發佈");
                document.getElementById("release-button").style.display = "none"; // 隱藏發佈按鈕
                document.getElementById("removed-from-market").style.display = "block"; // 顯示下架按鈕
            }

            let sopData = {

                FSopid: Number(sopId),
                FMemberId: Number(memberId),
                FSopType: Number(sopType),
                FPublisherId: Number(publisherId),
                FSopName: sopName,
                FSopDescription: sopDescription,
                FPubContent: pubContent,
                /*FPubSopImagePath: pubImage,*/
                FReleaseStatus: sopReleaseStatus,
                FIsRelease: Boolean(isRelease === "true" || isRelease === "1"), // 使用 Boolean() 正確轉換布林值
                FCompanySize: companySizeSelect,
                FDepartment: sopDepartment,
                FCustomer: sopCustomer,
                FBusiness: sopBusiness,
                FJobItemId: Number(hiddenJobItemId),
                FIndustryId: Number(hiddenIndustryId),
                FPrice: parseFloat(sopPrice),   // 使用 parseFloat 轉換為浮點數
                FSalePoints: parseFloat(sopSalePoints), // 使用 parseFloat 轉換為浮點數
                FReleaseTime: releaseTime,
            };

            console.log('儲存的資料:', sopData);

            try {
                const response = await fetch(`${baseAddress}/api/TPublisherSop/${sopId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(sopData),
                });

                if (response.ok) {
                    alert('儲存完成，已成功發佈');
                    document.getElementById("isRelease").value = "true"; // 更新前端的 `isRelease` 值 ( 因為是PubCanva才有的值，所以需要在這裡設定 )
                } else {
                    const errorText = await response.text();
                    console.error('儲存失敗：', errorText);
                }
            } catch (error) {
                console.error('儲存 TSop 資料過程中出現錯誤：', error);
                alert('發生錯誤，請稍後重試。');
            }
        };


    


}//releaseButton-release()的結尾





