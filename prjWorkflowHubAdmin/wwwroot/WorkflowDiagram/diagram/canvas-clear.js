import { CanvasSmbl } from '../infrastructure/canvas-smbl.js'; // 導入 Canvas 的符號
import { PathSmbl } from '../shapes/path-smbl.js'; // 導入 Path (路徑) 的符號
import { ShapeSmbl } from '../shapes/shape-smbl.js'; // 導入 Shape (形狀) 的符號

/**
 * 清除畫布上的所有元素
 * @param {CanvasElement} canvas - 自定義的 Canvas 元素
 */
export function canvasClear(canvas) {
    // 當畫布中還有子元素時，持續迭代
	while (canvas.firstChild) {
        // 嘗試刪除每個子元素。如果子元素具有 ShapeSmbl 或 PathSmbl 符號，調用其 del() 方法
		(canvas.firstChild[ShapeSmbl] || canvas.firstChild[PathSmbl]).del();
	}
    // 將畫布移動回原點 (0, 0)，並且可能應用某種縮放比例（這裡是 1）
	canvas[CanvasSmbl].move(0, 0, 1);
}

/**
 * 清除畫布上的選中狀態
 * @param {CanvasElement} canvas - 自定義的 Canvas 元素
 */
export function canvasSelectionClear(canvas) {
    // 檢查畫布是否有選擇清理功能，若有則調用 selectClear 方法來清除選中狀態
	if (canvas[CanvasSmbl].selectClear) { 
        canvas[CanvasSmbl].selectClear(); 
    }
}

/**
 * 為畫布設置選中狀態的清除函數
 * @param {CanvasElement} canvas - 自定義的 Canvas 元素
 * @param {()=>void} clearFn - 清除選中狀態的函數
 */
export function canvasSelectionClearSet(canvas, clearFn) {
    // 將傳遞進來的 clearFn 設置為畫布的 selectClear 函數，這樣可以自定義畫布的選中狀態清除行為
	canvas[CanvasSmbl].selectClear = clearFn;
}

/** 
 * 定義 CanvasElement 類型，用於幫助提供類型提示和檢查
 * @typedef { import('../infrastructure/move-scale-applay.js').CanvasElement } CanvasElement 
 */
