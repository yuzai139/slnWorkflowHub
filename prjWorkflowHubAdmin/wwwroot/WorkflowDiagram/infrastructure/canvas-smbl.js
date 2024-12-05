// 定義一個符號，用於標識與畫布相關的數據
export const CanvasSmbl = Symbol('Canvas'); // Canvas 的唯一符號，用於在元素上存儲和標識畫布相關的數據

/**
 * 定義一個 Point 類型，表示一個包含 x 和 y 坐標的點
 * @typedef { {x:number, y:number} } Point
 */

/**
 * 定義 CanvasData 類型，用於描述畫布的位置信息和縮放比例
 * @typedef {{position:Point, scale:number, cell: number}} CanvasData
 * - position: 畫布上的位置，為一個 Point 類型的對象
 * - scale: 畫布縮放比例
 * - cell: 畫布上的單元格數量或單元格大小
 */

/** @typedef {SVGGElement & { [CanvasSmbl]?: Canvas }} CanvasElement */

/**
@typedef {{
	move?(x:number, y:number, scale:number): void
	data: CanvasData
	// TODO: it is not infrastructure methods -> shouldn't be here
	selectClear?(): void
	shapeMap: Record<number, import("../shapes/shape-type-map").ShapeType>
}} Canvas
*/
