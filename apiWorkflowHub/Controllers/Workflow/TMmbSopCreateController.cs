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
    public class TMmbSopCreateController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TMmbSopCreateController(SOPMarketContext context)
        {
            _context = context;
        }

        // POST: api/TMmbSopCreate      //新建工作流程
        [HttpPost("{memberId}")]
        public async Task<ActionResult<TMemberSopDTO>> SopMemberCreate(int memberId)
        {
            TMemberSopDTO tSopDTO = new TMemberSopDTO();

            // 創建一個新的 SOP 預設資料
            var tSop = new TSop
            {
                FMemberId = memberId,
                FSopType = CSopDictionary.MemberSopType,
                FSopName = "未命名工作流程",
                FFileStatus = "啟用中",
                FEditTime = "-",
                FJobItemId = 0,
                FIndustryId = 0,
                FCompanySize = "請選擇",
                FSharePermission = "限自己編輯",

            };

            // 將 SOP 加入資料庫
            _context.TSops.Add(tSop);
            await _context.SaveChangesAsync();


            //添加附件的新資料
            TSopAffix tSopAffix = new TSopAffix
            {
                FSopid = tSop.FSopid
            };

            // 將 SopAffix 加入資料庫
            _context.TSopAffixes.Add(tSopAffix);
            await _context.SaveChangesAsync();

            // 更新 DTO 的 ID 並返回
            tSopDTO.FSopid = tSop.FSopid;
            tSopDTO.FMemberId = tSop.FMemberId;
            tSopDTO.FSopType = tSop.FSopType;
            tSopDTO.FSopName = tSop.FSopName;
            tSopDTO.FFileStatus = tSop.FFileStatus;
            tSopDTO.FEditTime = tSop.FEditTime;
            tSopDTO.FJobItemId = tSop.FJobItemId;
            tSopDTO.FIndustryId = tSop.FIndustryId;
            
            return tSopDTO;
        }

        // GET: api/TMmbSopCreate/
        [HttpGet("{sopid}")]  //把pub和mmb的sop資料都拿出來看看
        public async Task<ActionResult<TSopDTO>> GetTSopByMemberId(int sopid)
        {
            // 使用 LINQ 查詢 TSops 表，並與 TSopJobItems 和 TSopIndustries 表進行 Join，並取第一筆符合條件的資料
            var tSop = await _context.TSops
                .Where(p => p.FSopid == sopid) // && p.FSopType == CSopDictionary.MemberSopType
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
                    (sj, industry) => new TSopDTO        // 映射到 TMemberSopDTO
                    {
                        // TSops 表中的欄位
                        FSopid = sj.sop.FSopid,                // SOP ID
                        FMemberId = sj.sop.FMemberId,          // 會員 ID
                        FSopType = sj.sop.FSopType,            // SOP 類型
                        FSopName = sj.sop.FSopName,            // SOP 名稱
                        FSopFlowImagePath = sj.sop.FSopFlowImagePath, // SOP 流程圖路徑
                        FJobItemId = sj.sop.FJobItemId,        // 職業項目 ID
                        FIndustryId = sj.sop.FIndustryId,      // 行業 ID
                        FBusiness = sj.sop.FBusiness,
                        FCustomer = sj.sop.FCustomer,
                        FCompanySize = sj.sop.FCompanySize,
                        FSopDescription = sj.sop.FSopDescription,
                        FDepartment = sj.sop.FDepartment,
                        FShareUrl = sj.sop.FShareUrl,
                        FEditTime = sj.sop.FEditTime,
                        FSharePermission = sj.sop.FSharePermission,
                        FFileStatus = sj.sop.FFileStatus,      // 檔案狀態

                        //以下是PublisherSOP的欄位
                        FPublisherId = sj.sop.FPublisherId,
                        FPubContent = sj.sop.FPubContent,
                        FPubSopImagePath = sj.sop.FPubSopImagePath,
                        FReleaseTime = sj.sop.FReleaseTime,
                        FReleaseStatus = sj.sop.FReleaseStatus,
                        FProductUrl = sj.sop.FProductUrl,
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

        // POST: api/TMmbSopCreate/CreateAndRetrieve/{memberId}
        [HttpPost("CreateAndRetrieve/{memberId}")]
        public async Task<IActionResult> CreateAndRetrieve(int memberId)
        {
            // 1. 呼叫 SopMemberCreate 方法創建一個新的 SOP
            var createResult = await SopMemberCreate(memberId);

            // 檢查創建是否成功
            if (createResult.Result is BadRequestObjectResult)
            {
                return BadRequest("創建 SOP 失敗");
            }

            // 2. 取得創建後的 SOP ID
            int newSopId = createResult.Value.FSopid;

            // 3. 呼叫 GetTSopByMemberId 方法來查詢新創建的 SOP 的完整資訊
            var retrieveResult = await GetTSopByMemberId(newSopId);

            // 檢查查詢是否成功
            if (retrieveResult.Result is NotFoundObjectResult)
            {
                return NotFound("找不到新創建的 SOP 記錄");
            }

            // 4. 返回查詢結果與前端頁面 URL
            return Ok(new
            {
                data = retrieveResult.Value,
                redirectUrl = Url.Action("SopMemberEdit", "SopMember", new { sopId = newSopId })
            });
        }


        ////購買Sop後複製到會員Sop
        //[HttpPost("CopyPurchasedSops")] //POST: api/TMmbSopCreate/CopyPurchasedSops
        //public async Task<IActionResult> CopyPurchasedSops(int memberId, [FromBody] int[] sopIds)
        //{
        //    try
        //    {
        //        // 1. 驗證傳入的 `sopIds` 陣列和 `memberId`
        //        if (sopIds == null || !sopIds.Any() || memberId <= 0)
        //        {
        //            return BadRequest("請提供有效的 sopId 陣列和 memberId。");
        //        }

        //        // 2. 查詢符合的 SOP 資料
        //        var existingSops = await _context.TSops
        //            .Where(s => sopIds.Contains(s.FSopid))
        //            .ToListAsync();

        //        // 3. 若沒有符合的 SOP，返回 NotFound
        //        if (!existingSops.Any())
        //        {
        //            return NotFound("沒有找到符合的 SOP 資料。");
        //        }

        //        // 4. 複製 SOP 資料並更新 `SopName` 和 `MemberId`
        //        var newSops = new List<TSop>();
        //        foreach (var sop in existingSops)
        //        {
        //            var newSop = new TSop
        //            {
        //                FMemberId = memberId, // 使用傳入的 MemberId
        //                FSopName = sop.FSopName + "(已購買的商品)", // SopName 加上後綴
        //                FSopType = CSopDictionary.MemberSopType,
        //                FSopDescription = sop.FSopDescription,
        //                FSopFlowImagePath = sop.FSopFlowImagePath,
        //                FJobItemId = sop.FJobItemId,
        //                FIndustryId = sop.FIndustryId,
        //                FBusiness = sop.FBusiness,
        //                FCustomer = sop.FCustomer,
        //                FCompanySize = sop.FCompanySize,
        //                FDepartment = sop.FDepartment,
        //                //FShareUrl = sop.FShareUrl,
        //                FSharePermission = "限自己編輯",
        //                FFileStatus = "啟用中",
        //            };
        //            newSops.Add(newSop);
        //        }

        //        // 5. 批次新增複製的 SOP 資料
        //        _context.TSops.AddRange(newSops);
        //        await _context.SaveChangesAsync();

        //        // 回傳新增成功的結果
        //        return Ok(new { Message = "SOP 資料已成功複製。", 複製數量 = newSops.Count });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"發生錯誤：{ex.Message}");
        //    }
        //}



    }
}
