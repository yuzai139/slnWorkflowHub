import { ceil, child, positionSet, svgTxtFarthestPoint } from '../infrastructure/util.js'; // 引入工具函數
import { shapeCreate } from './shape-evt-proc.js'; // 引入形狀創建功能

/**
 * 創建一個圓形元素
 * @param {CanvasElement} canvas - 畫布元素
 * @param {CircleData} circleData - 圓形的數據
 */
export function circle(canvas, circleData) {
	const templ = `
		<circle data-key="outer" data-evt-no data-evt-index="2" r="72" fill="transparent" stroke-width="0" />
		<circle data-key="main" r="48" fill="#ff6600" stroke="#fff" stroke-width="1" />
		<text data-key="text" x="0" y="0" text-anchor="middle" style="pointer-events: none;" fill="#fff">&nbsp;</text>`;

	const shape = shapeCreate(canvas, circleData, templ,
		{
			right: { dir: 'right', position: { x: 48, y: 0 } },
			left: { dir: 'left', position: { x: -48, y: 0 } },
			bottom: { dir: 'bottom', position: { x: 0, y: 48 } },
			top: { dir: 'top', position: { x: 0, y: -48 } }
		},
		// onTextChange
		txtEl => {
			const newRadius = textElRadius(txtEl, 48, 24); // 計算新半徑
			if (newRadius !== circleData.r) {
				circleData.r = newRadius; // 更新圓的半徑
				resize(); // 重新調整圓的大小
			}
		});

	function resize() {
		shape.cons.right.position.x = circleData.r;
		shape.cons.left.position.x = -circleData.r;
		shape.cons.bottom.position.y = circleData.r;
		shape.cons.top.position.y = -circleData.r;

		for (const connectorKey in shape.cons) {
			positionSet(child(shape.el, connectorKey), shape.cons[connectorKey].position);
		}

		radiusSet(shape.el, 'outer', circleData.r + 24); // 設置外圓半徑
		radiusSet(shape.el, 'main', circleData.r); // 設置主圓半徑
		shape.draw(); // 繪製形狀
	}

	if (!!circleData.r && circleData.r !== 48) { resize(); } else { shape.draw(); }

	return shape.el;
}

/**
 * 設置圓的半徑
 * @param {Element} svgGrp - SVG 元素
 * @param {string} key - 圓的 key
 * @param {number} r - 圓的半徑
 */
function radiusSet(svgGrp, key, r) { 
	/** @type {SVGCircleElement} */
	(child(svgGrp, key)).r.baseVal.value = r; 
}

/**
 * 計算圓中包含所有文本的最小半徑
 * @param {SVGTextElement} textEl - 包含文本的 SVG 元素
 * @param {*} minR - 最小半徑
 * @param {*} step - 半徑的增量
 */
function textElRadius(textEl, minR, step) {
	const farthestPoint = svgTxtFarthestPoint(textEl); // 計算文本中最遠的點
	return ceil(minR, step, Math.sqrt(farthestPoint.x ** 2 + farthestPoint.y ** 2)); // 計算必要的半徑
}

/** @typedef { {x:number, y:number} } Point */
/** @typedef { import('../infrastructure/canvas-smbl.js').CanvasElement } CanvasElement */
/** @typedef { import('./shape-evt-proc').CanvasData } CanvasData */
/** @typedef { import('./shape-evt-proc').ConnectorsData } ConnectorsData */
/** @typedef { {type:number, position: Point, title?: string, styles?: string[], r?:number} } CircleData */
