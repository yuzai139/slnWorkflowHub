using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.Models.Workflow;
using System.IO;
using Microsoft.Extensions.Logging;

namespace prjWorkflowHubAdmin.Controllers.Workflow
{
    [Route("api/[controller]")]
    [EnableCors("All")] // 確保允許 CORS
    [ApiController]
    public class SopPublisherApiController : ControllerBase
    {
        private readonly SOPMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<SopPublisherApiController> _logger;

        public SopPublisherApiController(SOPMarketContext context, IWebHostEnvironment webHostEnvironment, ILogger<SopPublisherApiController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        private string CopyImageAndRename(string fileName)
        {
            // 檔案的來源路徑
            var sourcePath = Path.Combine(_webHostEnvironment.WebRootPath, "Workflow", "SopImages", fileName);

            // 檢查來源檔案是否存在
            if (!System.IO.File.Exists(sourcePath))  // 在這裡加上 System.IO
            {
                throw new FileNotFoundException("找不到來源檔案", sourcePath);
            }

            // 產生新的唯一檔名（使用 Guid）
            var newFileName = $"{Guid.NewGuid()}.png";
            var destinationPath = Path.Combine(_webHostEnvironment.WebRootPath, "Workflow", "SopImages", newFileName);

            try
            {
                // 複製檔案到新路徑
                System.IO.File.Copy(sourcePath, destinationPath);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "複製檔案時發生錯誤。");
                throw;
            }

            // 回傳新的檔名或完整路徑（依需求）
            return newFileName; // 或者 return destinationPath;
        }



        //// POST: api/SopPublisherApi   //複製會員Sop到發佈者Sop
        //[HttpPost("{sopid}/{publisherId}")]
        //public async Task<ActionResult<TSop>> CopySopBySopid(int sopid, int publisherId)
        //{
        //    var tSop = await _context.TSops
        //    .FirstOrDefaultAsync(p => p.FSopid == sopid);

        //    // 如果查詢結果為空，返回 NotFound 錯誤
        //    if (tSop == null)
        //    {
        //        return NotFound("找不到符合條件的 SOP 記錄");
        //    }

        //    //複製流程圖圖片和檔名 - 開始
        //    string newSopFlowImagePath = "";

        //    if (tSop.FSopFlowImagePath != null)
        //    {
        //        newSopFlowImagePath = CopyImageAndRename(tSop.FSopFlowImagePath);
        //    }
        //    else
        //    {
        //        _logger.LogWarning("沒有舊的 SOP Image 存在");
        //    }
        //    //複製流程圖圖片和檔名 - 結束


        //    // 創建一個新的 SOP 預設資料
        //    TSop tCopySop = new TSop
        //    {
        //        FMemberId = tSop.FMemberId,          // 會員 ID
        //        FPublisherId = publisherId,
        //        FSopType = CSopDictionary.PublishSopType,   // SOP 類型
        //        FSopName = tSop.FSopName + " 複製",  // SOP 名稱
        //        FSopFlowImagePath = newSopFlowImagePath,  // SOP 流程圖路徑
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
        //        FReleaseTime = "-",
        //        FReleaseStatus = "未發佈",
        //        FIsRelease = tSop.FIsRelease,
        //        FPrice = 0,
        //        FSalePoints = 0,

        //    };

        //    if (tCopySop.FMemberId != 0)
        //    {
        //        // 將 SOP 加入資料庫
        //        _context.TSops.Add(tCopySop);
        //        await _context.SaveChangesAsync();

        //        return Ok(true);
        //    }
        //    return BadRequest("無法複製 SOP。");
        //}



        
        [HttpPost("{sopid}/{publisherId}")] // POST: api/SopPublisherApi   //複製會員Sop到發佈者Sop
        public async Task<ActionResult<TSop>> CopySopBySopId(int sopid, int publisherId)
        {
            var tSop = await _context.TSops.FirstOrDefaultAsync(p => p.FSopid == sopid);

            if (tSop == null)
            {
                return NotFound("找不到符合條件的 SOP 記錄");
            }

            // 複製流程圖圖片和檔名
            string newSopFlowImagePath = tSop.FSopFlowImagePath != null
                ? CopyImageAndRename(tSop.FSopFlowImagePath)
                : "";

            // 創建一個新的 SOP
            TSop tCopySop = new TSop
            {
                FMemberId = tSop.FMemberId,
                FPublisherId = publisherId,
                FSopType = CSopDictionary.PublishSopType,
                FSopName = tSop.FSopName + " 複製",
                FSopFlowImagePath = newSopFlowImagePath,
                FPubSopImagePath = "",
                FJobItemId = tSop.FJobItemId,
                FIndustryId = tSop.FIndustryId,
                FBusiness = tSop.FBusiness,
                FCustomer = tSop.FCustomer,
                FCompanySize = tSop.FCompanySize,
                FSopDescription = tSop.FSopDescription,
                FPubContent = tSop.FPubContent,
                FDepartment = tSop.FDepartment,
                FProductUrl = "",
                FReleaseTime = "-",
                FReleaseStatus = "未發佈",
                FIsRelease = tSop.FIsRelease,
                FPrice = 0,
                FSalePoints = 0,
            };

            _context.TSops.Add(tCopySop);
            await _context.SaveChangesAsync();

            // 使用新 SOP 的 ID 複製附件
            await CopyAttachments(sopid, tCopySop.FSopid);

            return Ok(true);
        }

        /// <summary>
        /// 複製附件文件並儲存到新 SOP 的附件資料夾
        /// </summary>
        /// <param name="oldSopId">舊 SOP ID</param>
        /// <param name="newSopId">新 SOP ID</param>
        private async Task CopyAttachments(int oldSopId, int newSopId) //複製附件的方法
        {
            var oldAttachments = await _context.TSopAffixes.Where(a => a.FSopid == oldSopId).ToListAsync();
            var newAttachments = new List<TSopAffix>();
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Workflow", "SopAffix", $"sop_{newSopId}");

            // 確保新 SOP 的附件目錄存在
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // 如果沒有附件，或存在沒有名稱的附件，則添加空附件資料
            if (!oldAttachments.Any() || oldAttachments.All(a => string.IsNullOrEmpty(a.FAffixName)))
            {
                var emptyAttachment = new TSopAffix
                {
                    FSopid = newSopId,
                    FAffixName = "",  // 空附件名
                    FAffixPath = ""   // 空附件路徑
                };
                _context.TSopAffixes.Add(emptyAttachment);
                await _context.SaveChangesAsync();
                return; // 無需繼續執行複製附件
            }

            foreach (var oldAttachment in oldAttachments)
            {
                // 生成新的文件名稱並複製文件
                var newFileName = $"{Guid.NewGuid()}_{oldAttachment.FAffixName}";
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Workflow", "SopAffix", $"sop_{oldSopId}", oldAttachment.FAffixPath);
                var newFilePath = Path.Combine(uploadFolder, newFileName);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Copy(oldFilePath, newFilePath);
                }

                // 建立新附件記錄
                newAttachments.Add(new TSopAffix
                {
                    FSopid = newSopId,
                    FAffixName = oldAttachment.FAffixName,
                    FAffixPath = newFileName
                });
            }

            // 將新附件資料保存到資料庫
            _context.TSopAffixes.AddRange(newAttachments);
            await _context.SaveChangesAsync();
        }



        ////Post: api/SopPublisherApi/CopyPurchasedSops 購買Sop後複製到會員Sop
        //[HttpPost("CopyPurchasedSops/{memberId}")] // POST: api/SopPublisherApi/CopyPurchasedSops
        //public async Task<IActionResult> CopyPurchasedSops(int memberId, [FromBody] int[] sopIds)
        //{
        //    if (sopIds == null || !sopIds.Any() || memberId <= 0)
        //    {
        //        return BadRequest("請提供有效的 sopId 陣列和 memberId。");
        //    }

        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        // 1. 查詢符合的 SOP 資料
        //        var existingSops = await _context.TSops
        //            .Where(s => sopIds.Contains(s.FSopid))
        //            .ToListAsync();

        //        if (!existingSops.Any())
        //        {
        //            return NotFound("沒有找到符合的 SOP 資料。");
        //        }

        //        // 2. 複製 SOP 資料並更新 `SopName` 和 `MemberId`
        //        var newSops = new List<TSop>();
        //        foreach (var sop in existingSops)
        //        {
        //            string newSopFlowImagePath = null;

        //            if (!string.IsNullOrEmpty(sop.FSopFlowImagePath))
        //            {
        //                try
        //                {
        //                    newSopFlowImagePath = CopyImageAndRename(sop.FSopFlowImagePath);
        //                }
        //                catch (Exception imgEx)
        //                {
        //                    _logger.LogWarning(imgEx, $"無法複製圖片，FSopid: {sop.FSopid}");
        //                }
        //            }

        //            var newSop = new TSop
        //            {
        //                FMemberId = memberId,
        //                FSopName = $"{sop.FSopName}(已購買的商品)",
        //                FSopType = CSopDictionary.MemberSopType,
        //                FSopDescription = sop.FSopDescription,
        //                FSopFlowImagePath = newSopFlowImagePath,
        //                FJobItemId = sop.FJobItemId,
        //                FIndustryId = sop.FIndustryId,
        //                FBusiness = sop.FBusiness,
        //                FCustomer = sop.FCustomer,
        //                FCompanySize = sop.FCompanySize,
        //                FDepartment = sop.FDepartment,
        //                FSharePermission = "限自己編輯",
        //                FFileStatus = "啟用中",
        //                FEditTime = "-",
        //            };
        //            newSops.Add(newSop);
        //        }

        //        // 3. 批次新增複製的 SOP 資料
        //        await _context.TSops.AddRangeAsync(newSops);
        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        // 回傳新增成功的結果和新增 SOP ID 清單
        //        return Ok(new
        //        {
        //            Message = "SOP 資料已成功複製。",
        //            新增項目數 = newSops.Count,
        //            新增SopIds = newSops.Select(ns => ns.FSopid)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "發生錯誤");
        //        return StatusCode(500, $"發生錯誤：{ex.Message}");
        //    }
        //}


        [HttpPost("CopyPurchasedSops/{memberId}")] // POST: api/SopPublisherApi/CopyPurchasedSops  //購買Sop後複製到會員Sop
        public async Task<IActionResult> CopyPurchasedSops(int memberId, [FromBody] int[] sopIds)
        {
            if (sopIds == null || !sopIds.Any() || memberId <= 0)
            {
                return BadRequest("請提供有效的 sopId 陣列和 memberId。");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 查詢符合的 SOP 資料
                var existingSops = await _context.TSops
                    .Where(s => sopIds.Contains(s.FSopid))
                    .ToListAsync();

                if (!existingSops.Any())
                {
                    return NotFound("沒有找到符合的 SOP 資料。");
                }

                // 儲存新 SOP 的清單
                var newSops = new List<TSop>();

                foreach (var sop in existingSops)
                {
                    string newSopFlowImagePath = null;

                    if (!string.IsNullOrEmpty(sop.FSopFlowImagePath))
                    {
                        try
                        {
                            newSopFlowImagePath = CopyImageAndRename(sop.FSopFlowImagePath);
                        }
                        catch (Exception imgEx)
                        {
                            _logger.LogWarning(imgEx, $"無法複製圖片，FSopid: {sop.FSopid}");
                        }
                    }

                    var newSop = new TSop
                    {
                        FMemberId = memberId,
                        FSopName = $"{sop.FSopName}(已購買的商品)",
                        FSopType = CSopDictionary.MemberSopType,
                        FSopDescription = sop.FSopDescription,
                        FSopFlowImagePath = newSopFlowImagePath,
                        FJobItemId = sop.FJobItemId,
                        FIndustryId = sop.FIndustryId,
                        FBusiness = sop.FBusiness,
                        FCustomer = sop.FCustomer,
                        FCompanySize = sop.FCompanySize,
                        FDepartment = sop.FDepartment,
                        FSharePermission = "限自己編輯",
                        FFileStatus = "啟用中",
                        FEditTime = "-",
                    };

                    // 將新 SOP 添加到資料庫並保存更改，以獲取新 SOP 的 ID
                    _context.TSops.Add(newSop);
                    await _context.SaveChangesAsync();

                    // 呼叫方法以複製附件至新 SOP
                    await CopyAttachments(sop.FSopid, newSop.FSopid);

                    newSops.Add(newSop);
                }

                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "SOP 資料已成功複製。",
                    新增項目數 = newSops.Count,
                    新增SopIds = newSops.Select(ns => ns.FSopid)
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "發生錯誤");
                return StatusCode(500, $"發生錯誤：{ex.Message}");
            }
        }



        // PUT: api/SopPublisherApi/UploadImage/{sopId}  // 上傳發佈者圖片
        [HttpPut("UploadImage/{sopId}")]
        public async Task<IActionResult> UploadImage(int sopId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Ok(new { message = "無新發佈者sop圖片上傳，SOP 資料維持不變" });

            var tSop = await _context.TSops.FindAsync(sopId);
            if (tSop == null)
                return NotFound("SOP 不存在。");

            // 檢查是否有舊的圖片
            if (!string.IsNullOrEmpty(tSop.FPubSopImagePath))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Workflow", "PublishImages", tSop.FPubSopImagePath);
                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath); // 刪除舊圖片
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"刪除舊圖片失敗: {ex.Message}");
                    }
                }
            }

            // 生成新的唯一檔名
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Workflow", "PublishImages");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var newFilePath = Path.Combine(uploadFolder, uniqueFileName);

            // 儲存新圖片到指定路徑
            try
            {
                await using var fileStream = new FileStream(newFilePath, FileMode.Create);
                await file.CopyToAsync(fileStream);

                // 更新資料庫中的 SOP 資料
                tSop.FPubSopImagePath = uniqueFileName;
                //_context.Entry(tSop).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"發佈者Sop圖片儲存失敗: {ex.Message}");
            }

            return Ok(new { message = "發佈者Sop圖片已成功上傳", imagePath = uniqueFileName });
        }


        // 檢查 SOP 是否存在
        private bool TSopExists(int id)
        {
            return _context.TSops.Any(e => e.FSopid == id);
        }



    }
}
