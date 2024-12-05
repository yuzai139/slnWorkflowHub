// 引入各種模組和函數
import { moveEvtMobileFix } from '../infrastructure/move-evt-mobile-fix.js'; // 修正移動事件的模組
import { CanvasSmbl } from '../infrastructure/canvas-smbl.js'; // 提供畫布相關功能的符號
import { moveScaleApplay } from '../infrastructure/move-scale-applay.js'; // 處理移動和縮放的模組
import { evtRouteApplay } from '../infrastructure/evt-route-applay.js'; // 事件路由應用
import { tipShow, uiDisable } from '../ui/ui.js'; // 提示顯示和 UI 禁用功能
import { srvGet } from '../diagram/dgrm-srv.js'; // 服務端獲取圖表數據
import { deserialize } from '../diagram/dgrm-serialization.js'; // 反序列化圖表數據
import { copyPastApplay, groupSelectApplay } from '../diagram/group-select-applay.js'; // 複製、粘貼和組選功能
import { shapeTypeMap } from '../shapes/shape-type-map.js'; // 圖形類型映射
import '../ui/menu.js'; // 引入菜單 UI
import '../ui/shape-menu.js'; // 引入形狀菜單 UI
import './member-form2.js'; //跳出複製連結的按鈕
import './form-selection.js'; //表單-下拉選單接API 
import './modal-job.js';
import './modal-Industry.js';

// 取得畫布元素並初始化畫布符號
// @ts-ignore
/** @type {import('../infrastructure/canvas-smbl.js').CanvasElement} */ const canvas = document.getElementById('canvas');
canvas[CanvasSmbl] = {
    data: {
        position: { x: 0, y: 0 }, // 畫布初始位置
        scale: 1, // 初始縮放比例
        cell: 24 // 單元格大小
    },
    shapeMap: shapeTypeMap(canvas) // 形狀類型映射初始化
};

// 為畫布和事件設置移動和縮放的修正
moveEvtMobileFix(canvas.ownerSVGElement); // 修正移動事件，特別是對於移動設備
evtRouteApplay(canvas.ownerSVGElement); // 應用事件路由
copyPastApplay(canvas); // 啟用複製和粘貼功能
groupSelectApplay(canvas); // 啟用組選功能，必須在移動縮放之前
moveScaleApplay(canvas); // 啟用移動和縮放功能

// 初始化菜單和形狀菜單
/** @type { import('../ui/menu.js').Menu } */(document.getElementById('menu')).init(canvas);
/** @type { import('../ui/shape-menu.js').ShapeMenu } */(document.getElementById('menu-shape')).init(canvas);

// 根據 URL 的參數載入圖表
let url = new URL(window.location.href);
if (url.searchParams.get('k')) { // 如果 URL 中有參數 'k'
    uiDisable(true); // 禁用 UI
    srvGet(url.searchParams.get('k')).then(appData => { // 從服務器獲取圖表數據
        url.searchParams.delete('k');
        if (deserialize(canvas, appData)) { // 將數據反序列化並載入畫布
            tipShow(false); // 隱藏提示
        }
        history.replaceState(null, null, url); // 更新 URL 並移除參數
        uiDisable(false); // 啟用 UI
        url = null;
    });
} else { 
    url = null; 
}
