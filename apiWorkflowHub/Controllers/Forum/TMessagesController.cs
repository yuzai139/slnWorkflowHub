using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Forum;
using Microsoft.AspNetCore.Cors;

namespace apiWorkflowHub.Controllers.Forum
{
    [Route("api/[controller]")]
    [EnableCors("ALL")] // 啟用 CORS
    [ApiController]
    public class TMessagesController : ControllerBase   
    {
        private readonly SOPMarketContext _context;

        public TMessagesController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TMessages
       [HttpGet]
public async Task<ActionResult<object>> GetTMessages([FromQuery] int? articleId)
{
    try 
    {
        var query = _context.TMessages
            .Include(m => m.FArticle)
            .Include(m => m.FMember)
            .AsQueryable();

        if (articleId.HasValue)
        {
            query = query.Where(m => m.FArticleId == articleId);
        }

        // 取得所有符合條件的留言，不進行分頁
        var messages = await query
            .OrderByDescending(m => m.FCreatedAt)
            .Select(m => DTMessage.FromEntity(m))
            .ToListAsync();

        // 回傳資料和總筆數
        return Ok(new { 
            success = true,
            data = messages,
            totalCount = messages.Count
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"獲取留言時發生錯誤: {ex.Message}");
        return StatusCode(500, new { 
            success = false, 
            message = "獲取留言時發生錯誤 ，請稍後再試" 
        });
    }
}

        // GET: api/TMessages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DTMessage>> GetTMessage(int id)
        {
            var tMessage = await _context.TMessages
                .Include(m => m.FArticle)
                .Include(m => m.FMember)
                .FirstOrDefaultAsync(m => m.FMessageId == id);

            if (tMessage == null)
            {
                return NotFound();
            }

            return Ok(DTMessage.FromEntity(tMessage));
        }


// PUT: api/TMessages/5
[HttpPut("{id}")]
public async Task<ActionResult<object>> PutTMessage(int id, [FromBody] DTMessage dtMessage, [FromQuery] int memberId)
{
    try
    {
        if (id != dtMessage.FMessageId)
        {
            return BadRequest(new { success = false, message = "留言ID不符" });
        }

        var tMessage = await _context.TMessages.FindAsync(id);
        if (tMessage == null)
        {
            return NotFound(new { success = false, message = "找不到此留言" });
        }

        // 驗證是否為留言作者
        if (tMessage.FMemberId != memberId)
        {
            return StatusCode(403, new { success = false, message = "您沒有權限編輯此留言" });
        }

        // 驗證留言內容
        if (string.IsNullOrWhiteSpace(dtMessage.FMessageContent))
        {
            return BadRequest(new { success = false, message = "留言內容不能為空" });
        }

        if (dtMessage.FMessageContent.Length > 5000)
        {
            return BadRequest(new { success = false, message = "留言內容不能超過5000字" });
        }

        // 更新留言內容
        tMessage.FMessageContent = dtMessage.FMessageContent.Trim();
        tMessage.FUpdatedAt = DateTime.Now;

        _context.Entry(tMessage).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        // 重新查詢更新後的留言
        var updatedMessage = await _context.TMessages
            .Include(m => m.FArticle)
            .Include(m => m.FMember)
            .FirstOrDefaultAsync(m => m.FMessageId == id);

        return Ok(new { 
            success = true, 
            message = "留言更新成功",
            data = DTMessage.FromEntity(updatedMessage)
        });
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!TMessageExists(id))
        {
            return NotFound(new { success = false, message = "找不到此留言" });
        }
        Console.WriteLine("發生並行更新衝突");
        return StatusCode(500, new { success = false, message = "更新留言時發生衝突，請重試" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"更新留言時發生錯誤: {ex.Message}");
        return StatusCode(500, new { success = false, message = "更新留言時發生錯誤，請稍後再試" });
    }
}
      
      // POST: api/TMessages
[HttpPost]
public async Task<ActionResult<DTMessage>> PostTMessage(DTMessageCreate dtMessage)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 檢查文章是否存在
        var article = await _context.TArticles.FindAsync(dtMessage.FArticleId);
        if (article == null)
        {
            return BadRequest("文章不存在");
        }

        // 檢查會員是否存在
        var member = await _context.TMembers.FindAsync(dtMessage.FMemberId);
        if (member == null)
        {
            return BadRequest("會員不存在");
        }

        var tMessage = new TMessage
        {
            FArticleId = dtMessage.FArticleId,
            FMemberId = dtMessage.FMemberId,
            FMessageContent = dtMessage.FMessageContent?.Trim() ?? string.Empty,
            FCreatedAt = DateTime.Now,
            FUpdatedAt = DateTime.Now
        };

        _context.TMessages.Add(tMessage);
        await _context.SaveChangesAsync();

        // 重新查詢完整的留言資料
        var savedMessage = await _context.TMessages
            .Include(m => m.FArticle)
            .Include(m => m.FMember)
            .FirstOrDefaultAsync(m => m.FMessageId == tMessage.FMessageId);

        var result = DTMessage.FromEntity(savedMessage);
        return CreatedAtAction(nameof(GetTMessage), new { id = tMessage.FMessageId }, result);
    }
    catch (DbUpdateException ex)
    {
        // 記錄詳細的資料庫錯誤
        var innerException = ex.InnerException?.Message ?? "無詳細錯誤訊息";
        Console.WriteLine($"資料庫錯誤: {ex.Message}, 內部錯誤: {innerException}");
        return StatusCode(500, $"資料庫錯誤: {innerException}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"一般錯誤: {ex.Message}");
        return StatusCode(500, $"系統錯誤: {ex.Message}");
    }
}
        // DELETE: api/TMessages/5
     // DELETE: api/TMessages/5
[HttpDelete("{id}")]
public async Task<ActionResult<object>> DeleteTMessage(int id, [FromQuery] int memberId)
{
    try
    {
        var tMessage = await _context.TMessages
            .Include(m => m.FMember)
            .FirstOrDefaultAsync(m => m.FMessageId == id);

        if (tMessage == null)
        {
            return NotFound(new { success = false, message = "找不到此留言" });
        }

        // 驗證是否為留言作者
        if (tMessage.FMemberId != memberId)
        {
            return StatusCode(403, new { success = false, message = "您沒有權限刪除此留言" });
        }

        _context.TMessages.Remove(tMessage);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "留言已成功刪除" });
    }
    catch (Exception ex)
    {
        // 記錄錯誤
        Console.WriteLine($"刪除留言時發生錯誤: {ex.Message}");
        return StatusCode(500, new { success = false, message = "刪除留言時發生錯誤，請稍後再試" });
    }
}
        private bool TMessageExists(int id)
        {
            return _context.TMessages.Any(e => e.FMessageId == id);
        }
    }
}
