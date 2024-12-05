import { copyAndPast } from '../diagram/group-select-applay.js'; // 引入複製和粘貼功能
import { classAdd, classDel, clickForAll, listen, classSingleAdd, evtTargetAttr } from '../infrastructure/util.js'; // 引入工具函數
import { PathSmbl } from './path-smbl.js'; // 引入路徑符號

// 定義一個自定義元素 PathSettings，用於設置路徑樣式
export class PathSettings extends HTMLElement {
	/**
 	 * @param {CanvasElement} canvas - 畫布元素
	 * @param {PathElement} pathEl - 路徑元素
	 */
	constructor(canvas, pathEl) {
		super();
		/** @private */
		this._pathEl = pathEl;

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

			.stroke1 [data-cmd-arg="1"],
			.stroke2 [data-cmd-arg="2"],
			.stroke3 [data-cmd-arg="3"]
			{ fill-opacity: 1; stroke-opacity: 1; }
		</style>
		<ap-shape-edit id="edit" edit-btn="true">
			<div class="ln">
				<svg data-cmd data-cmd-arg="1" viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3V4zm0 15h14v2H3v-2zm0-5h18v2H3v-2zm0-5h14v2H3V9z" fill="rgb(52,71,103)"/></svg>
				<svg data-cmd data-cmd-arg="2" viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3v-2zm2 15h14v2H5v-2zm-2-5h18v2H3v-2zm2-5h14v2H5V9z" fill="rgb(52,71,103)"/></svg>
				<svg data-cmd data-cmd-arg="3" viewBox="0 0 24 24" width="24" height="24"><path fill="none" d="M0 0h24v24H0z"/><path d="M3 4h18v2H3v-2zm4 15h14v2H7v-2zm-4-5h18v2H3v-2zm4-5h14v2H7V9z" fill="rgb(52,71,103)"/></svg>
			</div>
		</ap-shape-edit>`;

		const pathData = /** @type {import('./path.js').PathData} */(this._pathEl[PathSmbl].data);

		const editEl = shadow.getElementById('edit');
		classAdd(editEl, `stroke-${pathData.a}`);

		// colors, del
		listen(editEl, 'cmd', /** @param {CustomEvent<{cmd:string, arg:string}>} evt */ evt => {
			switch (evt.detail.cmd) {
				case 'style': classSingleAdd(this._pathEl, pathData, 'cl-', evt.detail.arg); break;
				case 'del': this._pathEl[PathSmbl].del(); break;
				case 'copy': copyAndPast(this._canvas, [this._pathEl]); break;
			}
		});

		// path align
		clickForAll(shadow, '[data-cmd]', evt => {
			const alignNew = /** @type {1|2|3} */(Number.parseInt(evtTargetAttr(evt, 'data-cmd-arg')));
			if (alignNew === pathData.a) { return; }

			const alignOld = pathData.a;

			// apply path align to shape
			pathData.a = alignNew;
			this._pathEl[PathSmbl].draw();

			// highlight path align btn in settings panel
			classDel(editEl, `stroke-${alignOld}`);
			classAdd(editEl, `stroke-${pathData.a}`);
		});
	}
}
customElements.define('ap-path-settings', PathSettings); // 註冊自定義元素 'ap-path-settings'


/** @typedef { import('./path-smbl').PathElement } PathElement */
/** @typedef { import('../infrastructure/canvas-smbl.js').CanvasElement } CanvasElement */
