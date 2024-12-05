// 定義一個名為 PathSmbl 的符號，用於標識與路徑相關的數據
export const PathSmbl = Symbol('path');

/** 
 * 定義了一個類型 PathElement，它是一個 SVGGraphicsElement 並且可能包含與 PathSmbl 符號關聯的 Path 資料
 * @typedef {SVGGraphicsElement & { [PathSmbl]?: import("./path").Path }} PathElement 
 */
