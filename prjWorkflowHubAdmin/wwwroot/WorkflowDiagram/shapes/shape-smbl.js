// 定義一個符號，用於識別和存儲與形狀相關的數據
export const ShapeSmbl = Symbol('shape');

/** @typedef {SVGGraphicsElement & { [ShapeSmbl]?: import('./shape-evt-proc').Shape }} ShapeElement */
