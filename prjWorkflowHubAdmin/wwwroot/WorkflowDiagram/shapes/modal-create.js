/** @type {HTMLDivElement} */
let editModalDiv; // 用於存儲當前的編輯模態框元素

/**
 * 創建模態框，並將其顯示在指定的位置
 * @param {number} bottomX - 模態框底部左邊角的 X 座標
 * @param {number} bottomY - 模態框底部左邊角的 Y 座標
 * @param {HTMLElement} elem - 要放入模態框的元素
 */
export function modalCreate(bottomX, bottomY, elem) {
	editModalDiv = document.createElement('div'); // 創建一個新的 div 作為模態框
	editModalDiv.style.cssText = 'position: fixed; box-shadow: 0px 0px 58px 2px rgb(34 60 80 / 20%); border-radius: 16px; background-color: rgba(255,255,255, .9);';
	editModalDiv.append(elem); // 將傳入的元素添加到模態框中
	document.body.append(editModalDiv); // 將模態框添加到文檔的 body 中

	function position(btmX, btmY) {
		editModalDiv.style.left = `${btmX}px`; // 設置模態框的左邊距
		editModalDiv.style.top = `${window.scrollY + btmY - editModalDiv.getBoundingClientRect().height}px`; // 設置模態框的頂部距離
	}
	position(bottomX, bottomY); // 初始化模態框的位置

	return {
		/**
		 * 重新定位模態框
		 * @param {number} bottomX - 新的 X 座標
		 * @param {number} bottomY - 新的 Y 座標
		 */
		position,
		del: () => { editModalDiv.remove(); editModalDiv = null; } // 刪除模態框
	};
}

/**
 * 更改模態框的頂部位置
 * @param {number} dif - 頂部位置的改變量
 */
export function modalChangeTop(dif) {
	editModalDiv.style.top = `${editModalDiv.getBoundingClientRect().top + dif}px`; // 更改模態框的頂部位置
}
