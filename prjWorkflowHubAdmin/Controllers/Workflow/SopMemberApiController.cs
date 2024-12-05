using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjWorkflowHubAdmin.ContextModels;

namespace prjWorkflowHubAdmin.Controllers.Workflow
{
    [Route("api/[controller]")]
    [EnableCors("All")] // 確保允許 CORS
    [ApiController]
    public class SopMemberApiController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public SopMemberApiController(SOPMarketContext context)
        {
            _context = context;
        }


        // 刪除舊圖檔的方法
        private void DeleteOldImage(string ImagePath)
        {
            if (string.IsNullOrEmpty(ImagePath))
            {
                return;
            }

            // 將相對路徑轉換為伺服器上的實際絕對路徑
            string fullImagePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/Workflow/SopImages", Path.GetFileName(ImagePath));

            // 檢查檔案是否存在，如果存在則刪除
            if (System.IO.File.Exists(fullImagePath))
            {
                try
                {
                    System.IO.File.Delete(fullImagePath); // 刪除舊檔案
                    Console.WriteLine($"圖檔 {fullImagePath} 已被刪除。");
                }
                catch (Exception ex)
                {
                    // 處理刪除過程中的錯誤，例如：日誌記錄或返回錯誤信息
                    Console.WriteLine($"刪除檔案失敗: {ex.Message}");
                }
            }
        }


        //  api/SopMemberApi/id  //刪除工作流程 
        [HttpDelete("{sopId}")]
        public async Task<IActionResult> DeleteTSop(int sopId)
        {
            using (var db = new SOPMarketContext())
            {
                // 查詢 SOP
                var tsop = await _context.TSops.FindAsync(sopId);

                if (tsop == null)
                {
                    // 若找不到資料，返回 404 Not Found
                    return NotFound(new { message = "未找到指定的工作流程" });
                }

                // 移除 TSop 記錄
                db.TSops.Remove(tsop);
                db.SaveChanges();

                // 刪除舊圖片
                string deleteImagePath = tsop.FSopFlowImagePath;
                DeleteOldImage(deleteImagePath);

                // 回傳刪除成功的訊息
                return Ok(new { message = $"刪除 sopId = {tsop.FSopid} 成功" });

            }
        }


    }
}
