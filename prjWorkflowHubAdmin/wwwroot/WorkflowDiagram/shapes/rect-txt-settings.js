import { copyAndPast } from '../diagram/group-select-applay.js'; // 引入複製和粘貼功能模組
import { classAdd, classDel, clickForAll, listen, classSingleAdd, evtTargetAttr } from '../infrastructure/util.js'; // 引入工具函數模組，包括類操作和事件監聽
import { modalCreate } from './modal-create.js'; // 引入模態框創建功能
import { ShapeSmbl } from './shape-smbl.js'; // 引入形狀符號模組，用於管理形狀相關數據

/**
 * @param {import('../infrastructure/canvas-smbl.js').CanvasElement} canvas - 畫布元素
 * @param {import('./shape-smbl').ShapeElement} shapeElement - 要設置的矩形形狀元素
 * @param {number} bottomX - 面板底部左邊角的位置 X
 * @param {number} bottomY - 面板底部左邊角的位置 Y
 */
export const rectTxtSettingsPnlCreate = (canvas, shapeElement, bottomX, bottomY) =>
	modalCreate(bottomX, bottomY, new RectTxtSettings(canvas, shapeElement));

// 定義一個自定義元素 RectTxtSettings，用於設置矩形的文本屬性
class RectTxtSettings extends HTMLElement {
	/**
 	 * @param {import('../infrastructure/canvas-smbl.js').CanvasElement} canvas
	 * @param {import('./shape-smbl').ShapeElement} rectElement
	 */
	constructor(canvas, rectElement) {
		super();
		/** @private */
		this._rectElement = rectElement;

		/** @private */
		this._canvas = canvas;
	}

	connectedCallback() {
		const shadow = this.attachShadow({ mode: 'closed' });
		shadow.innerHTML = `
		<style>
			.ln { display: flex; }
			.ln > * {
				height: 24px;
				padding: 10px;
				fill-opacity: 0.3;
				stroke-opacity: 0.3;
			}
			[data-cmd] { cursor: pointer; }

			.ta-1 [data-cmd-arg="1"],
			.ta-2 [data-cmd-arg="2"],
			.ta-3 [data-cmd-arg="3"]
			{ fill-opacity: 1; stroke-opacity: 1; }
		</style>
		<ap-shape-edit id="edit" edit-btn="true">
			<div class="ln">
				<svg data-cmd data-cmd-arg="1" viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3V4zm0 15h14v2H3v-2zm0-5h18v2H3v-2zm0-5h14v2H3V9z" fill="rgb(52,71,103)"/></svg>
				<svg data-cmd data-cmd-arg="2" viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3V4zm2 15h14v2H5v-2zm-2-5h18v2H3v-2zm2-5h14v2H5V9z" fill="rgb(52,71,103)"/></svg>
				<svg data-cmd data-cmd-arg="3" viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3V4zm4 15h14v2H7v-2zm-4-5h18v2H3v-2zm4-5h14v2H7V9z" fill="rgb(52,71,103)"/></svg>
			</div>
		</ap-shape-edit>`; // 創建編輯區域的內部 HTML 和樣式

        const rectData = /** @type {import('./rect.js').RectData} */(this._rectElement[ShapeSmbl].data); // 獲取矩形的數據

        const editEl = shadow.getElementById('edit'); // 獲取編輯區域元素
        classAdd(editEl, `ta-${rectData.a}`); // 根據矩形的對齊屬性設置對應的樣式

        // 處理顏色、刪除等指令
        listen(editEl, 'cmd', /** @param {CustomEvent<{cmd:string, arg:string}>} evt */ evt => {
            switch (evt.detail.cmd) {
                case 'style': // 處理樣式更改指令
                    classSingleAdd(this._rectElement, rectData, 'cl-', evt.detail.arg);
                    break;
                case 'del': // 處理刪除指令
                    this._rectElement[ShapeSmbl].del();
                    break;
                case 'copy': // 處理複製指令
                    copyAndPast(this._canvas, [this._rectElement]);
                    break;
            }
        });

        // 處理文本對齊
        clickForAll(shadow, '[data-cmd]', evt => {
            const alignNew = /** @type {1|2|3} */(Number.parseInt(evtTargetAttr(evt, 'data-cmd-arg'))); // 獲取新的對齊方式
            if (alignNew === rectData.a) { return; } // 如果對齊方式未改變，則返回

            const alignOld = rectData.a; // 保存舊的對齊方式

            // 將新對齊方式應用到形狀
            rectData.a = alignNew;
            this._rectElement[ShapeSmbl].draw(); // 重新繪製形狀

            // 更新設置面板中的對齊按鈕狀態
            classDel(editEl, `ta-${alignOld}`);
            classAdd(editEl, `ta-${rectData.a}`);
        });
    }
}
customElements.define('ap-rect-txt-settings', RectTxtSettings); // 註冊自定義元素 'ap-rect-txt-settings'
