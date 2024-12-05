using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using Microsoft.AspNetCore.Cors;
using apiWorkflowHub.DTO.Forum;

namespace apiWorkflowHub.Controllers.Forum
{
    [Route("api/[controller]")]
    [EnableCors("ALL")] // 啟用 CORS
    [ApiController]
    public class TArticlesController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TArticlesController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TArticles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTArticle>>> GetTArticles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var articles = await _context.TArticles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var dtArticles = articles.Select(DTArticle.FromEntity).ToList();
            return dtArticles;
        }

        // GET: api/TArticles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DTArticle>> GetTArticle(int id)
        {
            var tArticle = await _context.TArticles.FindAsync(id);

            if (tArticle == null)
            {
                return NotFound();
            }

            return DTArticle.FromEntity(tArticle); // 使用 FromEntity 轉換
        }

        // PUT: api/TArticles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTArticle(int id, DTArticle dtArticle)
        {
            try
            {
                if (id != dtArticle.FArticleID)
                {
                    return BadRequest(new { message = "文章 ID 不符合" });
                }

                var tArticle = await _context.TArticles.FindAsync(id);
                if (tArticle == null)
                {
                    return NotFound(new { message = $"找不到 ID 為 {id} 的文章" });
                }

                // 更新實體
                tArticle.FArticleName = dtArticle.FArticleName;
                tArticle.FArticleContent = dtArticle.FArticleContent;
                tArticle.FCategoryNumber = dtArticle.FCategoryNumber;
                tArticle.FUpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.TArticles.AnyAsync(e => e.FArticleId == id))
                {
                    return NotFound(new { message = $"找不到 ID 為 {id} 的文章" });
                }
                throw;
            }
        }

        // POST: api/TArticles
        [HttpPost]
        public async Task<ActionResult<DTArticle>> PostTArticle([FromBody] DTArticleCreate createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest("文章資料不能為空");
                }

                // 使用資料庫追蹤來檢查實體狀態
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var tArticle = new TArticle
                    {
                        // 不設定 FArticleId
                        FArticleName = createDto.FArticleName.Trim(),
                        FArticleContent = createDto.FArticleContent.Trim(),
                        FCategoryNumber = createDto.FCategoryNumber,
                        FMemberId = createDto.FMemberID,
                        FCreatedAt = DateTime.Now,
                        FUpdatedAt = DateTime.Now
                    };

                    // 輸出實體狀態
                    Console.WriteLine($"新增前實體狀態: {_context.Entry(tArticle).State}");
                    
                    _context.TArticles.Add(tArticle);
                    
                    Console.WriteLine($"新增後實體狀態: {_context.Entry(tArticle).State}");
                    Console.WriteLine($"準備儲存的資料: {System.Text.Json.JsonSerializer.Serialize(tArticle)}");

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var result = DTArticle.FromEntity(tArticle);
                    return CreatedAtAction(nameof(GetTArticle), new { id = tArticle.FArticleId }, result);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤類型: {ex.GetType().Name}");
                Console.WriteLine($"錯誤訊息: {ex.Message}");
                Console.WriteLine($"內部錯誤: {ex.InnerException?.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");

                return StatusCode(500, new { 
                    message = "新增文章失敗", 
                    error = ex.Message,
                    details = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // DELETE: api/TArticles/5
        // DELETE: api/TArticles/5
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteTArticle(int id)
{
    try
    {
        // 開始資料庫交易
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 先找到文章
            var tArticle = await _context.TArticles
                .Include(a => a.TMessages) // 包含相關留言
                .FirstOrDefaultAsync(a => a.FArticleId == id);

            if (tArticle == null)
            {
                return NotFound(new { message = $"找不到 ID 為 {id} 的文章" });
            }

            // 如果有相關留言，先刪除留言
            if (tArticle.TMessages != null && tArticle.TMessages.Any())
            {
                _context.TMessages.RemoveRange(tArticle.TMessages);
            }

            // 刪除文章
            _context.TArticles.Remove(tArticle);
            
            // 儲存變更
            await _context.SaveChangesAsync();
            
            // 提交交易
            await transaction.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            // 發生錯誤時回滾交易
            await transaction.RollbackAsync();
            throw;
        }
    }
    catch (DbUpdateException ex)
    {
        // 資料庫更新錯誤
        Console.WriteLine($"資料庫更新錯誤: {ex.Message}");
        Console.WriteLine($"內部錯誤: {ex.InnerException?.Message}");
        return StatusCode(500, new { 
            message = "刪除文章時發生資料庫錯誤",
            error = ex.Message,
            details = ex.InnerException?.Message
        });
    }
    catch (Exception ex)
    {
        // 其他錯誤
        Console.WriteLine($"刪除文章時發生錯誤: {ex.Message}");
        Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
        return StatusCode(500, new { 
            message = "刪除文章時發生錯誤",
            error = ex.Message
        });

    }
}

        // GET: api/TArticles/category/{categoryNumber}
        [HttpGet("category/{categoryNumber}")]
        public async Task<ActionResult<IEnumerable<DTArticle>>> GetArticlesByCategory(int categoryNumber)
        {
            var articles = await _context.TArticles
                .Where(a => a.FCategoryNumber == categoryNumber)
                .ToListAsync();

            if (articles == null || !articles.Any())
            {
                return NotFound(); // 如果沒有找到文章，返回 404
            }

            var dtArticles = articles.Select(DTArticle.FromEntity).ToList(); // 使用 FromEntity 轉換
            return dtArticles; // 返回 DTO
        }

        // GET: api/TArticles/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DTArticle>>> SearchArticles([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("搜尋關鍵字不能為空");
            }

            var articles = await _context.TArticles
                .Where(a => a.FArticleName.Contains(keyword) || 
                           a.FArticleContent.Contains(keyword))
                .ToListAsync();

            var dtArticles = articles.Select(DTArticle.FromEntity).ToList();
            return dtArticles;
        }
    }
}
