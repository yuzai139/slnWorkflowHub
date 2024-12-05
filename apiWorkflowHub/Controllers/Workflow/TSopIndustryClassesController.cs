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
    [EnableCors("ALL")]
    [ApiController]
    public class TSopIndustryClassesController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TSopIndustryClassesController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TSopIndustryClasses
        [HttpGet]
        public async Task<IEnumerable<TSopIndustryClassDTO>> GetTSopIndustryClasses()
        {
            return await _context.TSopIndustryClasses
                .OrderBy(industryClass => industryClass.FIndustryClassSort)
                .Select(industryClass => new TSopIndustryClassDTO
                {
                    FIndustryClassId = industryClass.FIndustryClassId,
                    FIndustryClass = industryClass.FIndustryClass,
                    FIndustryClassSort = industryClass.FIndustryClassSort
                })
                .ToListAsync();
        }

        // GET: api/TSopIndustryClasses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TSopIndustryClassDTO>> GetTSopIndustryClass(int id)
        {
            var industryClass = await _context.TSopIndustryClasses
                .Where(ic => ic.FIndustryClassId == id)
                .Select(ic => new TSopIndustryClassDTO
                {
                    FIndustryClassId = ic.FIndustryClassId,
                    FIndustryClass = ic.FIndustryClass,
                    FIndustryClassSort = ic.FIndustryClassSort
                })
                .FirstOrDefaultAsync();

            if (industryClass == null)
            {
                return NotFound("找不到指定的行業分類");
            }

            return Ok(industryClass);
        }

        // PUT: api/TSopIndustryClasses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSopIndustryClass(int id, TSopIndustryClassDTO industryClassDTO)
        {
            if (id != industryClassDTO.FIndustryClassId)
            {
                return BadRequest("行業分類 ID 不匹配");
            }

            var existingIndustryClass = await _context.TSopIndustryClasses.FindAsync(id);
            if (existingIndustryClass == null)
            {
                return NotFound("行業分類不存在");
            }

            existingIndustryClass.FIndustryClass = industryClassDTO.FIndustryClass;
            existingIndustryClass.FIndustryClassSort = industryClassDTO.FIndustryClassSort;

            _context.Entry(existingIndustryClass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSopIndustryClassExists(id))
                {
                    return NotFound("行業分類已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TSopIndustryClasses
        [HttpPost]
        public async Task<ActionResult<TSopIndustryClassDTO>> PostTSopIndustryClass(TSopIndustryClassDTO industryClassDTO)
        {
            var newIndustryClass = new TSopIndustryClass
            {
                FIndustryClass = industryClassDTO.FIndustryClass,
                FIndustryClassSort = industryClassDTO.FIndustryClassSort
            };

            _context.TSopIndustryClasses.Add(newIndustryClass);
            await _context.SaveChangesAsync();

            industryClassDTO.FIndustryClassId = newIndustryClass.FIndustryClassId;

            return CreatedAtAction(nameof(GetTSopIndustryClass), new { id = newIndustryClass.FIndustryClassId }, industryClassDTO);
        }

        // DELETE: api/TSopIndustryClasses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSopIndustryClass(int id)
        {
            var industryClass = await _context.TSopIndustryClasses.FindAsync(id);
            if (industryClass == null)
            {
                return NotFound("行業分類不存在");
            }

            _context.TSopIndustryClasses.Remove(industryClass);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查行業分類是否存在
        private bool TSopIndustryClassExists(int id)
        {
            return _context.TSopIndustryClasses.Any(e => e.FIndustryClassId == id);
        }
    }
}
