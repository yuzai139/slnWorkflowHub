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
    public class TPubSopListController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TPubSopListController(SOPMarketContext context)
        {
            _context = context;
        }


        // GET: api/TPubSopList/{publisherId}
        [HttpGet("{publisherId}")]
        public async Task<ActionResult<IEnumerable<TPuberSopListDTO>>> GetTSopsByPublisherId(int publisherId, [FromQuery] string? keyword = null) // 設置 keyword 為可空
        {
            var query = _context.TSops
                .Where(p => p.FPublisherId == publisherId && p.FSopType == CSopDictionary.PublishSopType);

            // 如果有關鍵字，則增加篩選條件
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.FSopName.Contains(keyword));
            }

            var tSops = await query
                .Join(
                    _context.TSopJobItems,
                    sop => sop.FJobItemId,
                    job => job.FJobItemId,
                    (sop, job) => new { sop, job }
                )
                .Join(
                    _context.TSopIndustries,
                    sj => sj.sop.FIndustryId,
                    industry => industry.FIndustryId,
                    (sj, industry) => new TPuberSopListDTO
                    {
                        FSopid = sj.sop.FSopid,
                        FMemberId = sj.sop.FMemberId,
                        FPublisherId = sj.sop.FPublisherId,
                        FSopType = sj.sop.FSopType,
                        FSopName = sj.sop.FSopName,
                        FPubSopImagePath = sj.sop.FPubSopImagePath,
                        FJobItemId = sj.sop.FJobItemId,
                        FIndustryId = sj.sop.FIndustryId,
                        FReleaseStatus = sj.sop.FReleaseStatus,
                        FReleaseTime = sj.sop.FReleaseTime,
                        JobItem = sj.job.FJobItem,
                        Industry = industry.FIndustry
                    }
                )
                .ToListAsync();

            if (!tSops.Any())
            {
                return Ok(new { message = "找不到符合條件的 SOP 記錄" });
            }

            return tSops;
        }





        // GET: api/TPubSopList/{publisherId}
        //[HttpGet("{publisherId}")]
        //public async Task<ActionResult<IEnumerable<TPuberSopListDTO>>> GetTSopsByPublisherId(int publisherId)
        //{
        //    // 使用 LINQ 查詢 TSops 表，並與 TSopJobItems 和 TSopIndustries 表進行 Join
        //    var tSops = await _context.TSops
        //        .Where(p => p.FPublisherId == publisherId && p.FSopType == CSopDictionary.PublishSopType) // 根據 MemberId 和 FSopType 過濾
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
        //            (sj, industry) => new TPuberSopListDTO      // 映射到 TMembSopListDTO
        //            {
        //                // TSops 表中的欄位
        //                FSopid = sj.sop.FSopid,                // SOP ID
        //                FMemberId = sj.sop.FMemberId,          // 會員 ID
        //                FPublisherId = sj.sop.FPublisherId,
        //                FSopType = sj.sop.FSopType,            // SOP 類型
        //                FSopName = sj.sop.FSopName,            // SOP 名稱
        //                FPubSopImagePath = sj.sop.FPubSopImagePath, 
        //                FJobItemId = sj.sop.FJobItemId,        // 職業項目 ID
        //                FIndustryId = sj.sop.FIndustryId,      // 行業 ID
        //                FReleaseStatus = sj.sop.FReleaseStatus,
        //                FReleaseTime = sj.sop.FReleaseTime,


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
        //    return tSops;
        //}


        // GET: api/TPubSopList/MmbCopyList/{memberId}


        [HttpGet("MmbCopyList/{memberId}")]
        public async Task<ActionResult<IEnumerable<TMmbSopCopyListDTO>>> GetSopsByMemberId(int memberId)
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
                    (sj, industry) => new TMmbSopCopyListDTO      // 映射到 TMembSopListDTO
                    {
                        // TSops 表中的欄位
                        FSopid = sj.sop.FSopid,                // SOP ID
                        FMemberId = sj.sop.FMemberId,          // 會員 ID
                        FSopType = sj.sop.FSopType,            // SOP 類型
                        FSopName = sj.sop.FSopName,            // SOP 名稱
                        FJobItemId = sj.sop.FJobItemId,        // 職業項目 ID
                        FIndustryId = sj.sop.FIndustryId,      // 行業 ID
                        FFileStatus = sj.sop.FFileStatus,


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


        //// POST: api/TPubSopList/
        //[HttpPost("{sopid}/{publisherId}")] //要把這個方法移動到後台View專案中
        //public async Task<IActionResult> CopySopBySopid(int sopid, int publisherId)
        //{
        //    var tSop = await _context.TSops
        //        .Where(p => p.FSopid == sopid)
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
        //            (sj, industry) => new TPublisherSopDTO        // 映射到 TMemberSopDTO
        //            {
        //                // TSops 表中的欄位
        //                FSopid = sj.sop.FSopid,                // SOP ID
        //                FMemberId = sj.sop.FMemberId,          // 會員 ID
        //                FPublisherId = sj.sop.FPublisherId,
        //                FSopType = sj.sop.FSopType,            // SOP 類型
        //                FSopName = sj.sop.FSopName,            // SOP 名稱
        //                FSopFlowImagePath = sj.sop.FSopFlowImagePath, // SOP 流程圖路徑
        //                FPubSopImagePath = sj.sop.FPubSopImagePath,
        //                FJobItemId = sj.sop.FJobItemId,        // 職業項目 ID
        //                FIndustryId = sj.sop.FIndustryId,      // 行業 ID
        //                FBusiness = sj.sop.FBusiness,
        //                FCustomer = sj.sop.FCustomer,
        //                FCompanySize = sj.sop.FCompanySize,
        //                FSopDescription = sj.sop.FSopDescription,
        //                FPubContent = sj.sop.FPubContent,
        //                FDepartment = sj.sop.FDepartment,
        //                FProductUrl = sj.sop.FProductUrl,
        //                FReleaseTime = sj.sop.FReleaseTime,
        //                FReleaseStatus = sj.sop.FReleaseStatus,
        //                FIsRelease = sj.sop.FIsRelease,
        //                FPrice = sj.sop.FPrice,
        //                FSalePoints = sj.sop.FSalePoints,

        //                // 職業和行業
        //                JobItem = sj.job.FJobItem,
        //                Industry = industry.FIndustry
        //            }
        //        )
        //        .FirstOrDefaultAsync(); // 取第一筆資料

        //    // 如果查詢結果為空，返回 NotFound 錯誤
        //    if (tSop == null)
        //    {
        //        return NotFound("找不到符合條件的 SOP 記錄");
        //    }


        //    // 創建一個新的 SOP 預設資料
        //    TSop tCopySop = new TSop
        //    {
        //        FMemberId = tSop.FMemberId,          // 會員 ID
        //        FPublisherId = publisherId,
        //        FSopType = CSopDictionary.PublishSopType,   // SOP 類型
        //        FSopName = tSop.FSopName + " 複製",  // SOP 名稱
        //        FSopFlowImagePath = tSop.FSopFlowImagePath,  // SOP 流程圖路徑
        //        FPubSopImagePath = "",
        //        FJobItemId = tSop.FJobItemId,        // 職業項目 ID
        //        FIndustryId = tSop.FIndustryId,      // 行業 ID
        //        FBusiness = tSop.FBusiness,
        //        FCustomer = tSop.FCustomer,
        //        FCompanySize = tSop.FCompanySize,
        //        FSopDescription = tSop.FSopDescription,
        //        FPubContent = tSop.FPubContent,
        //        FDepartment = tSop.FDepartment,
        //        FProductUrl = "",
        //        FReleaseTime = "",
        //        FReleaseStatus = "未發佈",
        //        FIsRelease = tSop.FIsRelease,
        //        FPrice = 0,
        //        FSalePoints = 0,

        //    };

        //    if(tCopySop.FMemberId != 0)
        //    {
        //        // 將 SOP 加入資料庫
        //        _context.TSops.Add(tCopySop);
        //        await _context.SaveChangesAsync();

        //        return Ok($"複製 SOPID = {tCopySop.FSopid} 成功! ");
        //    }
        //    return BadRequest("無法複製 SOP。");
        //}




    }
}
