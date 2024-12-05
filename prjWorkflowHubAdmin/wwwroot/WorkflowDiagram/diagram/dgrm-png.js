import { CanvasSmbl } from '../infrastructure/canvas-smbl.js';
import { pngChunkGet, pngChunkSet } from '../infrastructure/png-chunk.js';
import { svgToPng } from '../infrastructure/svg-to-png.js';

/**
 * 將計算樣式轉為內聯樣式的函數
 * @param {HTMLElement} element
 */
function applyInlineStyles(element) {
    const computedStyle = window.getComputedStyle(element);
    for (const key of computedStyle) {
        element.style[key] = computedStyle.getPropertyValue(key);
    }
}

/**
 * 將外部 CSS 文件嵌入到 SVG 中的函數
 * @param {SVGSVGElement} svgElement
 * @param {string} cssText
 */
function embedStyles(svgElement, cssText) {
    const style = document.createElement('style');
    style.textContent = cssText;
    svgElement.insertBefore(style, svgElement.firstChild);
}

/**
 * 生成 PNG 的主函數
 * @param {CanvasElement} canvas
 * @param {string} dgrmChunkVal
 * @param {BlobCallback} callBack
 */
export function dgrmPngCreate(canvas, dgrmChunkVal, callBack) {
    const rectToShow = canvas.getBoundingClientRect();
    const svgVirtual = /** @type {SVGSVGElement} */(canvas.ownerSVGElement.cloneNode(true));
    svgVirtual.style.backgroundImage = null;
    svgVirtual.querySelectorAll('.select, .highlight').forEach(el => el.classList.remove('select', 'highlight'));

    const nonSvgElems = svgVirtual.getElementsByTagName('foreignObject');
    while (nonSvgElems[0]) { nonSvgElems[0].parentNode.removeChild(nonSvgElems[0]); }

    const canvasData = canvas[CanvasSmbl].data;

    // 從外部 CSS 文件嵌入樣式到 SVG 中
    fetch('~/WorkflowDiagram/styles/diagram-shap.css')
        .then(response => response.text())
        .then(cssText => {
            embedStyles(svgVirtual, cssText);

            const canvasElVirtual = /** @type{SVGGraphicsElement} */(svgVirtual.children[1]);
            const divis = 1 / canvasData.scale;
            canvasElVirtual.style.transform = `matrix(1, 0, 0, 1, 
                ${divis * (canvasData.position.x + 15 * canvasData.scale - rectToShow.x)},
                ${divis * (canvasData.position.y + 15 * canvasData.scale - rectToShow.y)})`;

            svgToPng(svgVirtual,
                { x: 0, y: 0, height: rectToShow.height / canvasData.scale + 30, width: rectToShow.width / canvasData.scale + 30 },
                3,
                async blob => callBack(await pngChunkSet(blob, 'dgRm', new TextEncoder().encode(dgrmChunkVal)))
            );
        })
        .catch(error => console.error('Failed to load and embed styles:', error));
}

/**
 * 解析 PNG 並返回特定 chunk 的值
 * @param {Blob} png
 * @returns {Promise<string|null>}
 */
export async function dgrmPngChunkGet(png) {
    const dgrmChunkVal = await pngChunkGet(png, 'dgRm');
    return dgrmChunkVal ? new TextDecoder().decode(dgrmChunkVal) : null;
}

/** @typedef { {x:number, y:number} } Point */
/** @typedef { import('../infrastructure/canvas-smbl.js').CanvasElement } CanvasElement */
