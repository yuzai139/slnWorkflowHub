// 引入必要的模組和功能
import { CanvasSmbl } from '../infrastructure/canvas-smbl.js'; // 畫布符號
import { pointInCanvas } from '../infrastructure/move-scale-applay.js'; // 獲取在畫布中的點
import { listen } from '../infrastructure/util.js'; // 事件監聽工具
import { tipShow } from './ui.js'; // 提示顯示功能

// 定義圖案形狀菜單類別，繼承自 HTMLElement
export class ShapeMenu extends HTMLElement {
	connectedCallback() {
		const shadow = this.attachShadow({ mode: 'closed' });// 使用 Shadow DOM 隱藏內部結構
		shadow.innerHTML =
			`<style>
			.menu {
				overflow-x: auto;
				padding: 0;
				position: fixed;
				bottom: 1.8%;                 /* 垂直居中 */
				left: 50%;                /* 水平居中 */
				transform: translate(-50%, -50%);  /* 將菜單的中心點置中 */
				box-shadow: 0px 0px 58px 2px rgba(34, 60, 80, 0.2);
				border-radius: 16px;
				background-color: rgba(255,255,255, .9);
			}

			.content {
				white-space: nowrap;
				display: flex;
				flex-direction: row;      /* 改為水平排列 */
			}
			
			[data-cmd] {
				cursor: pointer;
			}

			.menu svg { padding: 10px; }
			.stroke {
				stroke: #344767;
				stroke-width: 2px;
				fill: transparent;
			}

			.menu .big {
				width: 62px;
				min-width: 62px;
			}

			@media only screen and (max-width: 700px) {
				.menu {
					width: 100%;
					border-radius: 0;
					bottom: 0;
					display: flex;
					flex-direction: column;
					top: unset;
					left: unset;
					transform: unset;
				}

				.content {
					align-self: center;
					flex-direction: row;
				}
			}
		</style>
		<div id="menu" class="menu" style="touch-action: none;">
			<div class="content">
				<svg class="stroke" data-cmd="shapeAdd" data-cmd-arg="1" viewBox="0 0 24 24" width="24" height="24">
					<circle r="9" cx="12" cy="12"></circle>
				</svg>
				<svg class="stroke" data-cmd="shapeAdd" data-cmd-arg="4" viewBox="0 0 24 24" width="24" height="24">
					<path d="M2 12 L12 2 L22 12 L12 22 Z" stroke-linejoin="round"></path>
				</svg>
				<svg class="stroke" data-cmd="shapeAdd" data-cmd-arg="2" viewBox="0 0 24 24" width="24" height="24">
					<rect x="2" y="4" width="20" height="16" rx="3" ry="3"></rect>
				</svg>
				<svg data-cmd="shapeAdd" data-cmd-arg="0" viewBox="0 0 24 24" width="24" height="24">
					<path fill="none" d="M0 0h24v24H0z"/>
					<path d="M13 8v8a3 3 0 0 1-3 3H7.83a3.001 3.001 0 1 1 0-2H10a1 1 0 0 0 1-1V8a3 3 0 0 1 3-3h3V2l5 4-5 4V7h-3a1 1 0 0 0-1 1z" fill="rgba(52,71,103,1)"/>
				</svg>
				<svg data-cmd="shapeAdd" data-cmd-arg="3" viewBox="0 0 24 24" width="24" height="24">
					<path fill="none" d="M0 0h24v24H0z"/>
					<path d="M13 6v15h-2V6H5V4h14v2z" fill="rgba(52,71,103,1)"/>
				</svg>
			</div>
		</div>`;

		const menu = shadow.getElementById('menu');// 菜單元素
		menu.querySelectorAll('[data-cmd="shapeAdd"]').forEach(el => listen(el, 'pointerdown', this));// 為每個形狀按鈕添加事件監聽
		listen(menu, 'pointerleave', this);// 當鼠標離開菜單時觸發
		listen(menu, 'pointerup', this); // 當在菜單中釋放鼠標按鍵時觸發
		listen(menu, 'pointermove', this); // 當在菜單中移動鼠標時觸發
	};

	/** @param {CanvasElement} canvas */
	init(canvas) {
		/** @private */ this._canvas = canvas; // 儲存畫布元素
	}

	/** @param {PointerEvent & { currentTarget: Element }} evt */
	handleEvent(evt) {
		switch (evt.type) {
			case 'pointermove':
				if (!this._isNativePointerleaveTriggered) {
					// emulate pointerleave for mobile

					const pointElem = document.elementFromPoint(evt.clientX, evt.clientY);
					if (pointElem === this._pointElem) {
						return;
					}

					// pointerleave
					if (this._parentElem === this._pointElem) {
						// TODO: check mobile
						this._canvas.ownerSVGElement.setPointerCapture(evt.pointerId);
					}

					/**
					 * @type {Element}
					 * @private
					 */
					this._pointElem = pointElem;
				}
				break;
			case 'pointerleave':
				this._isNativePointerleaveTriggered = true;
				if (this._pressedShapeTemplKey != null) {
					// when shape drag out from menu panel
					this._shapeCreate(evt);
				}
				this._clean();
				break;
			case 'pointerdown':
				this._pressedShapeTemplKey = parseInt(evt.currentTarget.getAttribute('data-cmd-arg'));// 獲取按鈕的形狀類型

				// for emulate pointerleave
				this._parentElem = document.elementFromPoint(evt.clientX, evt.clientY);
				this._pointElem = this._parentElem;
				this._isNativePointerleaveTriggered = null;
				break;
			case 'pointerup':
				this._clean();
				break;
		}
	}

	/**
	 * @param {PointerEvent} evt
	 * @private
	 */
	_shapeCreate(evt) {
		tipShow(false);// 隱藏提示

		const evtPoint = pointInCanvas(this._canvas[CanvasSmbl].data, evt.clientX, evt.clientY); // 計算在畫布中的點

		//  TODO: create facktory map with increasing
		const shapeData = this._pressedShapeTemplKey === 0
			? /** @type {import('../shapes/path.js').PathData} */({
				s: { data: { dir: 'right', position: { x: evtPoint.x - 24, y: evtPoint.y } } },
				e: { data: { dir: 'right', position: { x: evtPoint.x + 24, y: evtPoint.y } } }
			})
			: {
				type: this._pressedShapeTemplKey,// 根據按下的按鈕類型創建形狀數據
				position: {
					x: evtPoint.x,
					y: evtPoint.y
				},
				title: 'Title'// 默認標題
			};

		const shapeEl = this._canvas[CanvasSmbl].shapeMap[this._pressedShapeTemplKey].create(shapeData);// 創建形狀元素
		this._canvas.append(shapeEl);// 將形狀添加到畫布中
		shapeEl.dispatchEvent(new PointerEvent('pointerdown', evt));
	}

	/** @private */
	_clean() {
		this._pressedShapeTemplKey = null;
		this._parentElem = null;
		this._pointElem = null;
	}
}
customElements.define('ap-menu-shape', ShapeMenu);// 註冊自定義元素 'ap-menu-shape'

/** @typedef { import('../shapes/shape-type-map.js').ShapeType } ShapeType */
/** @typedef { import('../infrastructure/canvas-smbl.js').CanvasElement } CanvasElement */
