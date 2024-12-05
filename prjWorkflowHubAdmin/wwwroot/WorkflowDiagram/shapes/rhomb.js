// 導入一些實用工具函數和方法
import { ceil, child, classAdd, positionSet, svgTxtFarthestPoint } from '../infrastructure/util.js';
import { shapeCreate } from './shape-evt-proc.js';

/**
 * 創建並返回一個菱形圖形元素
 * @param {CanvasElement} canvas - 目標畫布元素
 * @param {RhombData} rhombData - 菱形的數據，包括位置、類型等
 */
export function rhomb(canvas, rhombData) {
    // 定義菱形的 SVG 模板，包括外框、邊框、主體和文字元素
    const templ = `
        <path data-key="outer" data-evt-no data-evt-index="2" d="M-72 0 L0 -72 L72 0 L0 72 Z" stroke-width="0" fill="transparent" />
        <path data-key="border" d="M-39 0 L0 -39 L39 0 L0 39 Z" stroke-width="20" stroke="#fff" fill="transparent" stroke-linejoin="round" />
        <path data-key="main" d="M-39 0 L0 -39 L39 0 L0 39 Z" stroke-width="18" stroke-linejoin="round" stroke="#1D809F" fill="#1D809F" />
        <text data-key="text" x="0" y="0" text-anchor="middle" style="pointer-events: none;" fill="#fff">&nbsp;</text>`;

    // 使用 shapeCreate 函數創建一個新的菱形，並傳入畫布、數據和模板
    const shape = shapeCreate(canvas, rhombData, templ,
        {
            // 定義菱形的四個連接點，分別在右、左、下、上的相對位置
            right: { dir: 'right', position: { x: 48, y: 0 } },
            left: { dir: 'left', position: { x: -48, y: 0 } },
            bottom: { dir: 'bottom', position: { x: 0, y: 48 } },
            top: { dir: 'top', position: { x: 0, y: -48 } }
        },
        // 當文本內容改變時的回調函數
        txtEl => {
            // 根據文本元素計算新的菱形寬度
            const newWidth = ceil(96, 48, textElRhombWidth(txtEl) - 20); // -20 為實驗值，調整寬度
            // 如果新的寬度與當前的不同，更新數據並重新調整菱形尺寸
            if (newWidth !== rhombData.w) {
                rhombData.w = newWidth;
                resize();
            }
        });
    // 將 'shrhomb' 類添加到形狀元素，以標識這是菱形形狀
    classAdd(shape.el, 'shrhomb');

    /**
     * 調整菱形的大小和連接點位置
     */
    function resize() {
        // 計算當前寬度下的菱形連接點位置
        const connectors = rhombCalc(rhombData.w, 0);
        shape.cons.right.position.x = connectors.r.x;
        shape.cons.left.position.x = connectors.l.x;
        shape.cons.bottom.position.y = connectors.b.y;
        shape.cons.top.position.y = connectors.t.y;

        // 更新每個連接點的 DOM 位置
        for (const connectorKey in shape.cons) {
            positionSet(child(shape.el, connectorKey), shape.cons[connectorKey].position);
        }

        // 計算並設置主體和邊框的形狀
        const mainRhomb = rhombCalc(rhombData.w, 9);
        rhombSet(shape.el, 'main', mainRhomb);
        rhombSet(shape.el, 'border', mainRhomb);
        // 設置外框的形狀
        rhombSet(shape.el, 'outer', rhombCalc(rhombData.w, -24));

        // 繪製形狀
        shape.draw();
    }

    // 如果初始寬度已設置且不等於 96，則調整大小；否則直接繪製
    if (!!rhombData.w && rhombData.w !== 96) {
        resize();
    } else {
        shape.draw();
    }

    // 返回形狀元素
    return shape.el;
}

/**
 * 設置指定 SVG 群組中路徑的形狀
 * @param {Element} svgGrp - SVG 群組元素
 * @param {string} key - 路徑元素的數據鍵
 * @param {RhombPoints} rhomb - 菱形的點數據
 */
function rhombSet(svgGrp, key, rhomb) {
    /** @type {SVGPathElement} */(child(svgGrp, key)).setAttribute('d', `M${rhomb.l.x} ${rhomb.l.y} L${rhomb.t.x} ${rhomb.t.y} L${rhomb.r.x} ${rhomb.r.y} L${rhomb.b.x} ${rhomb.b.y} Z`);
}

/**
 * 根據寬度和邊距計算正方形菱形的四個頂點
 * 原點在菱形的中心
 * @param {number} width - 菱形的寬度
 * @param {number} margin - 邊距
 * @returns {RhombPoints} - 菱形的點數據
 */
function rhombCalc(width, margin) {
    const half = width / 2; // 計算寬度的一半
    const mrgnMinHalf = margin - half; // 邊距減去一半寬度
    const halfMinMrgn = half - margin; // 一半寬度減去邊距
    // 返回四個頂點的坐標
    return {
        l: { x: mrgnMinHalf, y: 0 }, // 左側點
        t: { x: 0, y: mrgnMinHalf }, // 上側點
        r: { x: halfMinMrgn, y: 0 }, // 右側點
        b: { x: 0, y: halfMinMrgn }  // 下側點
    };
}

/**
 * 計算包含所有文本的正方形菱形的寬度
 * 原點在菱形的中心
 * @param {SVGTextElement} textEl - 包含文本的 SVG 文本元素
 * @returns {number} - 菱形的寬度
 */
function textElRhombWidth(textEl) {
    // 計算文本的最遠點，以確定菱形的寬度
    const farthestPoint = svgTxtFarthestPoint(textEl);
    // 返回兩倍的最遠點的距離作為菱形的寬度
    return 2 * (Math.abs(farthestPoint.x) + Math.abs(farthestPoint.y));
}

/** @typedef { {x:number, y:number} } Point - 表示平面上的一個點 */
/** @typedef { import('../infrastructure/canvas-smbl.js').CanvasElement } CanvasElement - 畫布元素類型定義 */
/** @typedef { import('./shape-evt-proc').CanvasData } CanvasData - 畫布數據類型定義 */
/** @typedef { import('./shape-evt-proc').ConnectorsData } ConnectorsData - 連接器數據類型定義 */
/**
 * 菱形數據類型定義，包括類型、位置、標題、樣式和寬度
 * @typedef {{
 *   type:number, 
 *   position: Point, 
 *   title?: string, 
 *   styles?: string[], 
 *   w?:number
 * }} RhombData
 */
/** @typedef { { l:Point, t:Point, r:Point, b:Point } } RhombPoints - 表示菱形的四個頂點坐標 */
