using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Workflow;

namespace apiWorkflowHub.Controllers.Workflow
{
    [Route("api/[controller]")]
    [ApiController]
    public class TSopJobItemsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TSopJobItemsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TSopJobItems
        [HttpGet]
        public async Task<IEnumerable<TSopItemDTO>> GetTSopJobItems()
        {
            return await _context.TSopJobItems
                .OrderBy(item => item.FJobItemSort)
                .Select(item => new TSopItemDTO
                {
                    FJobItemId = item.FJobItemId,
                    FJobItem = item.FJobItem,
                    FJobId = item.FJobId,
                    FJobItemSort = item.FJobItemSort
                })
                .ToListAsync();
        }

        // GET: api/TSopJobItems/{jobId}
        [HttpGet("{jobId}")]
        public async Task<ActionResult<IEnumerable<TSopItemDTO>>> GetTSopJobItemsByJob(int jobId)
        {
            var items = await _context.TSopJobItems
                .Where(item => item.FJobId == jobId)
                .Select(item => new TSopItemDTO
                {
                    FJobItemId = item.FJobItemId,
                    FJobItem = item.FJobItem,
                    FJobId = item.FJobId,
                    FJobItemSort = item.FJobItemSort
                })
                .ToListAsync();

            if (!items.Any())
            {
                return NotFound("沒有找到符合條件的工作項目");
            }

            return Ok(items);
        }

        // PUT: api/TSopJobItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSopJobItem(int id, TSopItemDTO jobItemDTO)
        {
            if (id != jobItemDTO.FJobItemId)
            {
                return BadRequest("工作項目 ID 不匹配");
            }

            var existingJobItem = await _context.TSopJobItems.FindAsync(id);
            if (existingJobItem == null)
            {
                return NotFound("工作項目不存在");
            }

            existingJobItem.FJobItem = jobItemDTO.FJobItem;
            existingJobItem.FJobId = jobItemDTO.FJobId;
            existingJobItem.FJobItemSort = jobItemDTO.FJobItemSort;

            _context.Entry(existingJobItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSopJobItemExists(id))
                {
                    return NotFound("工作項目已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TSopJobItems
        [HttpPost]
        public async Task<ActionResult<TSopItemDTO>> PostTSopJobItem(TSopItemDTO jobItemDTO)
        {
            var newJobItem = new TSopJobItem
            {
                FJobItem = jobItemDTO.FJobItem,
                FJobId = jobItemDTO.FJobId,
                FJobItemSort = jobItemDTO.FJobItemSort
            };

            _context.TSopJobItems.Add(newJobItem);
            await _context.SaveChangesAsync();

            jobItemDTO.FJobItemId = newJobItem.FJobItemId;

            return CreatedAtAction(nameof(GetTSopJobItemsByJob), new { jobId = newJobItem.FJobId }, jobItemDTO);
        }

        // DELETE: api/TSopJobItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSopJobItem(int id)
        {
            var jobItem = await _context.TSopJobItems.FindAsync(id);
            if (jobItem == null)
            {
                return NotFound("工作項目不存在");
            }

            _context.TSopJobItems.Remove(jobItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查工作項目是否存在
        private bool TSopJobItemExists(int id)
        {
            return _context.TSopJobItems.Any(e => e.FJobItemId == id);
        }
    }
}
