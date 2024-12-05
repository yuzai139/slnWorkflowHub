import { CanvasSmbl } from '../infrastructure/canvas-smbl.js';
import { pngChunkGet, pngChunkSet } from '../infrastructure/png-chunk.js';
import { svgToPng } from '../infrastructure/svg-to-png.js';

/**
 * �N�p��˦��ର���p�˦������
 * @param {HTMLElement} element
 */
function applyInlineStyles(element) {
    const computedStyle = window.getComputedStyle(element);
    for (const key of computedStyle) {
        element.style[key] = computedStyle.getPropertyValue(key);
    }
}

/**
 * �N�~�� CSS ���O�J�� SVG �������
 * @param {SVGSVGElement} svgElement
 * @param {string} cssText
 */
function embedStyles(svgElement, cssText) {
    const style = document.createElement('style');
    style.textContent = cssText;
    svgElement.insertBefore(style, svgElement.firstChild);
}

/**
 * �ͦ� PNG ���D���
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

    // �q�~�� CSS ���O�J�˦��� SVG ��
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
 * �ѪR PNG �ê�^�S�w chunk ����
 * @param {Blob} png
 * @returns {Promise<string|null>}
 */
export async function dgrmPngChunkGet(png) {
    const dgrmChunkVal = await pngChunkGet(png, 'dgRm');
    return dgrmChunkVal ? new TextDecoder().decode(dgrmChunkVal) : null;
}

/** @typedef { {x:number, y:number} } Point */
/** @typedef { import('../infrastructure/canvas-smbl.js').CanvasElement } CanvasElement */
