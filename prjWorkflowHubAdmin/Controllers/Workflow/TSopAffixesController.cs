using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.ViewModels.Workflow;

namespace prjWorkflowHubAdmin.Controllers.Workflow
{
    [Route("api/[controller]")]
    [EnableCors("All")] // 確保允許 CORS
    [ApiController]
    public class TSopAffixesController : ControllerBase
    {
        private readonly SOPMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TSopAffixesController(SOPMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // 注入 IWebHostEnvironment
        }

        // 取得所有 SOP 附件列表
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CPubSopAffixesViewModel>>> GetTSopAffixes()
        {
            return await _context.TSopAffixes
                .Select(a => new CPubSopAffixesViewModel
                {
                    FSopaffixId = a.FSopaffixId,
                    FSopid = a.FSopid,
                    FAffixName = a.FAffixName,
                    FAffixPath = a.FAffixPath
                })
                .ToListAsync();
        }

        // 根據 ID 取得特定 SOP 附件
        [HttpGet("{id}")]
        public async Task<ActionResult<CPubSopAffixesViewModel>> GetTSopAffix(int id)
        {
            var tSopAffix = await _context.TSopAffixes.FindAsync(id);

            if (tSopAffix == null)
            {
                return NotFound();
            }

            // 將 TSopAffix 轉換為 ViewModel
            return new CPubSopAffixesViewModel
            {
                FSopaffixId = tSopAffix.FSopaffixId,
                FSopid = tSopAffix.FSopid,
                FAffixName = tSopAffix.FAffixName,
                FAffixPath = tSopAffix.FAffixPath
            };
        }




        [HttpGet("GetSopFiles/{sopId}")]
        public async Task<ActionResult<IEnumerable<CPubSopAffixesViewModel>>> GetSopFiles(int sopId)
        {
            try
            {
                var files = await _context.TSopAffixes
                    .Where(x => x.FSopid == sopId)
                    .Select(a => new CPubSopAffixesViewModel
                    {
                        FSopaffixId = a.FSopaffixId,
                        FSopid = a.FSopid,
                        FAffixName = a.FAffixName,
                        FAffixPath = $"/Workflow/SopAffix/sop_{a.FSopid}/{a.FAffixPath}" // 返回相對於網站根的路徑
                    })
                    .ToListAsync();

                if (files == null || !files.Any())
                    return NotFound("此 SOP 沒有相關文件。");

                return Ok(files);
            }
            catch (Exception ex)
            {
                // 可以記錄錯誤日誌，並返回一般性錯誤訊息給前端
                Console.WriteLine($"Error retrieving files: {ex.Message}");
                return StatusCode(500, "伺服器內部錯誤，請稍後再試。");
            }
        }




        // 更新指定 ID 的 SOP 附件
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSopAffix(int id, CPubSopAffixesViewModel model)
        {
            if (id != model.FSopaffixId)
            {
                return BadRequest();
            }

            var tSopAffix = await _context.TSopAffixes.FindAsync(id);
            if (tSopAffix == null)
            {
                return NotFound();
            }

            // 更新資料庫模型的屬性
            tSopAffix.FSopid = model.FSopid;
            tSopAffix.FAffixName = model.FAffixName;
            tSopAffix.FAffixPath = model.FAffixPath;

            _context.Entry(tSopAffix).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSopAffixExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        [HttpPut("UploadSopFile/{sopId}")]
        public async Task<IActionResult> UploadSopFile(int sopId, List<IFormFile> files)
        {
            const long maxFileSize = 20 * 1024 * 1024; // 單個文件大小上限 20 MB
            const int maxFileCount = 5; // 單次最多上傳文件數量上限 5 個

            if (files == null || files.Count == 0)
            {
                return Ok(new { message = "沒有選擇附件" });
            }

            // 驗證文件數量
            if (files.Count > maxFileCount)
            {
                return BadRequest($"每次最多只能上傳 {maxFileCount} 個附件。");
            }

            // 檢查 SOP 是否存在
            var sop = await _context.TSops.FindAsync(sopId);
            if (sop == null)
            {
                return NotFound("找不到對應的 SOP。");
            }

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Workflow", "SopAffix", $"sop_{sopId}");

            // 確保上傳目錄存在
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // 先刪除舊文件
            var existingFiles = _context.TSopAffixes.Where(a => a.FSopid == sopId).ToList();
            foreach (var file in existingFiles)
            {
                if (!string.IsNullOrEmpty(file.FAffixPath))
                {
                    var fullPath = Path.Combine(uploadFolder, file.FAffixPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            _context.TSopAffixes.RemoveRange(existingFiles);

            // 上傳新文件
            var newFiles = new List<TSopAffix>();
            foreach (var file in files)
            {
                // 檢查文件大小
                if (file.Length > maxFileSize)
                {
                    return BadRequest($"附件 '{file.FileName}' 超過了 20 MB 的大小限制。");
                }

                // 生成唯一的文件名稱，避免名稱衝突
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                // 儲存文件到硬碟
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // 構建文件資訊並加入列表
                newFiles.Add(new TSopAffix
                {
                    FSopid = sopId,
                    FAffixName = file.FileName,         // 原始文件名稱
                    FAffixPath = uniqueFileName // 儲存的相對路徑
                });
            }

            // 將新文件資訊儲存到資料庫
            _context.TSopAffixes.AddRange(newFiles);
            await _context.SaveChangesAsync();

            return Ok(new { message = "附件已成功上傳", fileCount = newFiles.Count });
        }




        // 新增 SOP 附件
        [HttpPost]
        public async Task<ActionResult<CPubSopAffixesViewModel>> PostTSopAffix(CPubSopAffixesViewModel model)
        {
            var tSopAffix = new TSopAffix
            {
                FSopaffixId = model.FSopaffixId,
                FSopid = model.FSopid,
                FAffixName = model.FAffixName,
                FAffixPath = model.FAffixPath
            };

            _context.TSopAffixes.Add(tSopAffix);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TSopAffixExists(tSopAffix.FSopaffixId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // 返回新創建的 ViewModel
            return CreatedAtAction("GetTSopAffix", new { id = tSopAffix.FSopaffixId }, model);
        }



        [HttpPost("UploadSopFile/{sopId}")]
        public async Task<IActionResult> UploadSopFile(int sopId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("請提供有效的文件。");

            // 生成唯一檔名
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", $"sop_{sopId}");

            // 確保目錄存在
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // 儲存檔案至硬碟
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 儲存文件資訊至資料庫
            var sopAffix = new TSopAffix
            {
                FSopid = sopId,
                FAffixName = file.FileName,
                FAffixPath = $"/uploads/sop_{sopId}/{uniqueFileName}"
            };
            _context.TSopAffixes.Add(sopAffix);
            await _context.SaveChangesAsync();

            return Ok(new { message = "文件已成功上傳", affixId = sopAffix.FSopaffixId });
        }




        // 刪除指定 ID 的 SOP 附件
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSopAffix(int id)
        {
            var tSopAffix = await _context.TSopAffixes.FindAsync(id);
            if (tSopAffix == null)
            {
                return NotFound();
            }

            _context.TSopAffixes.Remove(tSopAffix);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查是否存在指定 ID 的 SOP 附件
        private bool TSopAffixExists(int id)
        {
            return _context.TSopAffixes.Any(e => e.FSopaffixId == id);
        }
    }
}
