using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Workflow;
using apiWorkflowHub.Models.Workflow;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiWorkflowHub.Controllers.Workflow
{
    [Route("api/[controller]")]
    [EnableCors("ALL")] // 確保允許 CORS
    [ApiController]
    public class TMmbSopsListController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TMmbSopsListController(SOPMarketContext context)
        {
            _context = context;
        }

        //可以查關鍵字和MemberId的版本
        //// GET: api/TMmbSopsList/{memberId}
        //[HttpGet("{memberId}")]
        //public async Task<ActionResult<IEnumerable<TMembSopListDTO>>> GetTSopsByMemberId(int memberId, [FromQuery] CSopKeywordViewModels? keyword)
        //{
        //    // 獲取關鍵字的值
        //    string txtKeyword = keyword?.txtKeyword;

        //    // 使用 LINQ 查詢 TSops 表，並與 TSopJobItems 和 TSopIndustries 表進行 Join
        //    var query = _context.TSops
        //        .Where(p => p.FMemberId == memberId && p.FSopType == CSopDictionary.MemberSopType); // 根據 MemberId 和 FSopType 過濾

        //    // 若 txtKeyword 不為空，則加入 FSopName 的模糊查詢條件
        //    if (!string.IsNullOrEmpty(txtKeyword))
        //    {
        //        query = query.Where(p => p.FSopName.Contains(txtKeyword));
        //    }

        //    // 執行查詢，並進行 Join 操作以取得職業和行業的額外資料
        //    var tSops = await query
        //        .Join(
        //            _context.TSopJobItems,                     // 與 TSopJobItems 表 Join
        //            sop => sop.FJobItemId,                     // TSops 表的 FJobItemId
        //            job => job.FJobItemId,                     // TSopJobItems 表的 FJobItemId
        //            (sop, job) => new { sop, job }             // 結果包含 TSops 和 TSopJobItems 的資料
        //        )
        //        .Join(
        //            _context.TSopIndustries,                   // 與 TSopIndustries 表 Join
        //            sj => sj.sop.FIndustryId,                  // TSops 表的 FIndustryId
        //            industry => industry.FIndustryId,          // TSopIndustries 表的 FIndustryId
        //            (sj, industry) => new TMembSopListDTO      // 映射到 TMembSopListDTO
        //            {
        //                // TSops 表中的欄位
        //                FSopid = sj.sop.FSopid,                // SOP ID
        //                FMemberId = sj.sop.FMemberId,          // 會員 ID
        //                FSopType = sj.sop.FSopType,            // SOP 類型
        //                FSopName = sj.sop.FSopName,            // SOP 名稱
        //                FSopFlowImagePath = sj.sop.FSopFlowImagePath, // SOP 流程圖路徑
        //                FJobItemId = sj.sop.FJobItemId,        // 職業項目 ID
        //                FIndustryId = sj.sop.FIndustryId,      // 行業 ID
        //                FFileStatus = sj.sop.FFileStatus,      // 檔案狀態

        //                // 僅選取需要的 TSopJobItems 和 TSopIndustries 表中的欄位
        //                JobItem = sj.job.FJobItem,             // 職業名稱
        //                Industry = industry.FIndustry          // 行業名稱
        //            }
        //        )
        //        .ToListAsync();

        //    // 如果查詢結果為空，返回 NotFound 錯誤
        //    if (!tSops.Any())
        //    {
        //        return NotFound("找不到符合條件的 SOP 記錄");
        //    }

        //    // 返回查詢結果
        //    return Ok(tSops);
        //}



        // GET: api/TMmbSopsList/{memberId}
        [HttpGet("{memberId}")]
        public async Task<ActionResult<IEnumerable<TMembSopListDTO>>> GetTSopsByMemberId(int memberId)
        {
            // 使用 LINQ 查詢 TSops 表，並與 TSopJobItems 和 TSopIndustries 表進行 Join
            var tSops = await _context.TSops
                .Where(p => p.FMemberId == memberId && p.FSopType == CSopDictionary.MemberSopType) // 根據 MemberId 和 FSopType 過濾
                .Join(
                    _context.TSopJobItems,                     // 與 TSopJobItems 表 Join
                    sop => sop.FJobItemId,                     // TSops 表的 FJobItemId
                    job => job.FJobItemId,                     // TSopJobItems 表的 FJobItemId
                    (sop, job) => new { sop, job }             // 結果包含 TSops 和 TSopJobItems 的資料
                )
                .Join(
                    _context.TSopIndustries,                   // 與 TSopIndustries 表 Join
                    sj => sj.sop.FIndustryId,                  // TSops 表的 FIndustryId
                    industry => industry.FIndustryId,          // TSopIndustries 表的 FIndustryId
                    (sj, industry) => new TMembSopListDTO      // 映射到 TMembSopListDTO
                    {
                        // TSops 表中的欄位
                        FSopid = sj.sop.FSopid,                // SOP ID
                        FMemberId = sj.sop.FMemberId,          // 會員 ID
                        FSopType = sj.sop.FSopType,            // SOP 類型
                        FSopName = sj.sop.FSopName,            // SOP 名稱
                        FSopFlowImagePath = sj.sop.FSopFlowImagePath, // SOP 流程圖路徑
                        FJobItemId = sj.sop.FJobItemId,        // 職業項目 ID
                        FIndustryId = sj.sop.FIndustryId,      // 行業 ID
                        FFileStatus = sj.sop.FFileStatus,      // 檔案狀態
                        FEditTime = sj.sop.FEditTime,
                        FSharePermission = sj.sop.FSharePermission,
                        

                        // 僅選取需要的 TSopJobItems 和 TSopIndustries 表中的欄位
                        JobItem = sj.job.FJobItem,             // 職業名稱
                        Industry = industry.FIndustry          // 行業名稱
                    }
                )
                .ToListAsync();

            // 如果查詢結果為空，返回 NotFound 錯誤
            if (!tSops.Any())
            {
                return NotFound("找不到符合條件的 SOP 記錄");
            }

            // 返回查詢結果
            return tSops;
        }

    }
}
