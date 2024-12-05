using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Workflow;
using Microsoft.AspNetCore.Cors;

namespace apiWorkflowHub.Controllers.Workflow
{
    [Route("api/[controller]")]
    [EnableCors("ALL")] // 啟用 CORS
    [ApiController]
    public class TSopJobClassesController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TSopJobClassesController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TSopJobClasses
        [HttpGet]
        public async Task<IEnumerable<TSopJobClassDTO>> GetTSopJobClasses()
        {
            // 查詢所有 TSopJobClasses，按 FJobClassSort 排序並轉換為 DTO 格式
            return await _context.TSopJobClasses
                .OrderBy(jobClass => jobClass.FJobClassSort)
                .Select(jobClass => new TSopJobClassDTO
                {
                    FJobClassId = jobClass.FJobClassId,
                    FJobClass = jobClass.FJobClass,
                    FJobClassSort = jobClass.FJobClassSort
                })
                .ToListAsync();
        }

        // GET: api/TSopJobClasses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TSopJobClassDTO>> GetTSopJobClass(int id)
        {
            // 根據 FJobClassId 查詢特定職業分類
            var jobClass = await _context.TSopJobClasses.FindAsync(id);

            if (jobClass == null)
            {
                return NotFound("職業分類不存在");
            }

            // 將結果轉換為 DTO 格式並返回
            var jobClassDTO = new TSopJobClassDTO
            {
                FJobClassId = jobClass.FJobClassId,
                FJobClass = jobClass.FJobClass,
                FJobClassSort = jobClass.FJobClassSort
            };

            return jobClassDTO;
        }

        // PUT: api/TSopJobClasses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSopJobClass(int id, TSopJobClassDTO jobClassDTO)
        {
            // 檢查傳入的 ID 是否和 DTO 的 FJobClassId 相符
            if (id != jobClassDTO.FJobClassId)
            {
                return BadRequest("職業分類 ID 不匹配");
            }

            // 查詢現有的職業分類
            var existingJobClass = await _context.TSopJobClasses.FindAsync(id);
            if (existingJobClass == null)
            {
                return NotFound("職業分類不存在");
            }

            // 更新資料
            existingJobClass.FJobClass = jobClassDTO.FJobClass;
            existingJobClass.FJobClassSort = jobClassDTO.FJobClassSort;

            _context.Entry(existingJobClass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSopJobClassExists(id))
                {
                    return NotFound("職業分類已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TSopJobClasses
        [HttpPost]
        public async Task<ActionResult<TSopJobClassDTO>> PostTSopJobClass(TSopJobClassDTO jobClassDTO)
        {
            // 創建新的職業分類實體
            var newJobClass = new TSopJobClass
            {
                FJobClass = jobClassDTO.FJobClass,
                FJobClassSort = jobClassDTO.FJobClassSort
            };

            _context.TSopJobClasses.Add(newJobClass);
            await _context.SaveChangesAsync();

            // 返回新建的資源
            jobClassDTO.FJobClassId = newJobClass.FJobClassId;
            return CreatedAtAction(nameof(GetTSopJobClass), new { id = newJobClass.FJobClassId }, jobClassDTO);
        }

        // DELETE: api/TSopJobClasses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSopJobClass(int id)
        {
            var jobClass = await _context.TSopJobClasses.FindAsync(id);
            if (jobClass == null)
            {
                return NotFound("職業分類不存在");
            }

            _context.TSopJobClasses.Remove(jobClass);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查職業分類是否存在
        private bool TSopJobClassExists(int id)
        {
            return _context.TSopJobClasses.Any(e => e.FJobClassId == id);
        }
    }
}
