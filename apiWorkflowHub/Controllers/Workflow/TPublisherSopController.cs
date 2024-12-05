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
    public class TPublisherSopController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TPublisherSopController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TPubSop/
        [HttpGet("{sopid}")]
        public async Task<ActionResult<TPublisherSopDTO>> GetTPubSopBySopid(int sopid)
        {
            var tSop = await _context.TSops
                .Where(p => p.FSopid == sopid && p.FSopType == CSopDictionary.PublishSopType)
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
                    (sj, industry) => new TPublisherSopDTO        // 映射到 TMemberSopDTO
                    {
                        // TSops 表中的欄位
                        FSopid = sj.sop.FSopid,                // SOP ID
                        FMemberId = sj.sop.FMemberId,          // 會員 ID
                        FPublisherId = sj.sop.FPublisherId,
                        FSopType = sj.sop.FSopType,            // SOP 類型
                        FSopName = sj.sop.FSopName,            // SOP 名稱
                        FSopFlowImagePath = sj.sop.FSopFlowImagePath, // SOP 流程圖路徑
                        FPubSopImagePath = sj.sop.FPubSopImagePath,
                        FJobItemId = sj.sop.FJobItemId,        // 職業項目 ID
                        FIndustryId = sj.sop.FIndustryId,      // 行業 ID
                        FBusiness = sj.sop.FBusiness,
                        FCustomer = sj.sop.FCustomer,
                        FCompanySize = sj.sop.FCompanySize,
                        FSopDescription = sj.sop.FSopDescription,
                        FPubContent = sj.sop.FPubContent,
                        FDepartment = sj.sop.FDepartment,
                        FProductUrl = sj.sop.FProductUrl,
                        FReleaseTime = sj.sop.FReleaseTime,
                        FReleaseStatus = sj.sop.FReleaseStatus,
                        FIsRelease = sj.sop.FIsRelease,
                        FPrice = sj.sop.FPrice,
                        FSalePoints = sj.sop.FSalePoints,

                        // 職業和行業
                        JobItem = sj.job.FJobItem,
                        Industry = industry.FIndustry
                    }
                )
                .FirstOrDefaultAsync(); // 取第一筆資料

            // 如果查詢結果為空，返回 NotFound 錯誤
            if (tSop == null)
            {
                return NotFound("找不到符合條件的 SOP 記錄");
            }

            // 返回查詢結果
            return tSop;
        }


        // PUT: api/TPublisherSop/Sopid
        [HttpPut("{sopId}")]
        public async Task<IActionResult> PutTSop(int sopId, TPublisherSopDTO tSopDTO)
        {
            // 檢查 ModelState 是否有效
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 這樣可以返回具體的錯誤信息
            }


            // 檢查是否為正確的 SOP ID
            if (sopId != tSopDTO.FSopid)
            {
                return BadRequest("SOP ID 不匹配");
            }

            // 查詢現有的 SOP
            var tSop = await _context.TSops.FindAsync(sopId);
            if (tSop == null)
            {
                return NotFound("SOP 不存在");
            }

            // 更新實體資料
            tSop.FMemberId = tSopDTO.FMemberId;
            tSop.FPublisherId = tSopDTO.FPublisherId;
            tSop.FSopType = tSopDTO.FSopType;
            tSop.FSopName = tSopDTO.FSopName;
            tSop.FSopDescription = tSopDTO.FSopDescription;
            tSop.FPubContent = tSopDTO.FPubContent;
            //tSop.FSopFlowImagePath = tSopDTO.FSopFlowImagePath; //1不需要更新畫布
            //tSop.FPubSopImagePath = tSopDTO.FPubSopImagePath;
            tSop.FJobItemId = tSopDTO.FJobItemId;
            tSop.FIndustryId = tSopDTO.FIndustryId;
            tSop.FBusiness = tSopDTO.FBusiness;
            tSop.FCustomer = tSopDTO.FCustomer;
            tSop.FCompanySize = tSopDTO.FCompanySize;
            tSop.FDepartment = tSopDTO.FDepartment;
            tSop.FProductUrl = tSopDTO.FProductUrl;
            tSop.FReleaseTime = tSopDTO.FReleaseTime;
            tSop.FReleaseStatus = tSopDTO.FReleaseStatus;
            tSop.FIsRelease = tSopDTO.FIsRelease;
            tSop.FPrice = tSopDTO.FPrice;
            tSop.FSalePoints = tSopDTO.FSalePoints;

            // 標記狀態為已修改
            _context.Entry(tSop).State = EntityState.Modified;

            try
            {
                // 保存變更
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 檢查 SOP 是否仍存在
                if (!TSopExists(sopId))
                {
                    return NotFound("SOP 已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // PUT: api/TPublisherSop/PutReleaseStatus/Sopid
        [HttpPut("PutReleaseStatus/{sopId}")]
        public async Task<IActionResult> PutReleaseStatus(int sopId, TPublisherSopDTO tSopDTO)
        {
            // 檢查 ModelState 是否有效
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 這樣可以返回具體的錯誤信息
            }


            // 檢查是否為正確的 SOP ID
            if (sopId != tSopDTO.FSopid)
            {
                return BadRequest("SOP ID 不匹配");
            }

            // 查詢現有的 SOP
            var tSop = await _context.TSops.FindAsync(sopId);
            if (tSop == null)
            {
                return NotFound("SOP 不存在");
            }
            tSop.FReleaseStatus = tSopDTO.FReleaseStatus;
            tSop.FReleaseTime = tSopDTO.FReleaseTime;

            // 標記狀態為已修改
            _context.Entry(tSop).State = EntityState.Modified;

            try
            {
                // 保存變更
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 檢查 SOP 是否仍存在
                if (!TSopExists(sopId))
                {
                    return NotFound("SOP 已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // 檢查 SOP 是否存在
        private bool TSopExists(int id)
        {
            return _context.TSops.Any(e => e.FSopid == id);
        }



    }
}
