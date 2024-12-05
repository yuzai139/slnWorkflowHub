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
    public class TSopIndustriesController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TSopIndustriesController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TSopIndustries
        [HttpGet]
        public async Task<IEnumerable<TSopIndustryDTO>> GetTSopIndustries()
        {
            return await _context.TSopIndustries
                .OrderBy(industry => industry.FIndustrySort)
                .Select(industry => new TSopIndustryDTO
                {
                    FIndustryId = industry.FIndustryId,
                    FIndustry = industry.FIndustry,
                    FIndustryClassId = industry.FIndustryClassId,
                    FIndustrySort = industry.FIndustrySort
                })
                .ToListAsync();
        }

        // GET: api/TSopIndustries/{id}
        [HttpGet("{industryClassId}")]
        public async Task<ActionResult<IEnumerable<TSopIndustryDTO>>> GetTSopIndustry(int industryClassId)
        {
            var industrylist = await _context.TSopIndustries
                .Where(i => i.FIndustryClassId == industryClassId)
                .Select(i => new TSopIndustryDTO
                {
                    FIndustryId = i.FIndustryId,
                    FIndustry = i.FIndustry,
                    FIndustryClassId = i.FIndustryClassId,
                    FIndustrySort = i.FIndustrySort
                })
                .ToListAsync();

            if (industrylist == null)
            {
                return NotFound("找不到指定的行業");
            }

            return Ok(industrylist);
        }

        // PUT: api/TSopIndustries/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSopIndustry(int id, TSopIndustryDTO industryDTO)
        {
            if (id != industryDTO.FIndustryId)
            {
                return BadRequest("行業 ID 不匹配");
            }

            var existingIndustry = await _context.TSopIndustries.FindAsync(id);
            if (existingIndustry == null)
            {
                return NotFound("行業不存在");
            }

            existingIndustry.FIndustry = industryDTO.FIndustry;
            existingIndustry.FIndustryClassId = industryDTO.FIndustryClassId;
            existingIndustry.FIndustrySort = industryDTO.FIndustrySort;

            _context.Entry(existingIndustry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSopIndustryExists(id))
                {
                    return NotFound("行業已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TSopIndustries
        [HttpPost]
        public async Task<ActionResult<TSopIndustryDTO>> PostTSopIndustry(TSopIndustryDTO industryDTO)
        {
            var newIndustry = new TSopIndustry
            {
                FIndustry = industryDTO.FIndustry,
                FIndustryClassId = industryDTO.FIndustryClassId,
                FIndustrySort = industryDTO.FIndustrySort
            };

            _context.TSopIndustries.Add(newIndustry);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TSopIndustryExists(newIndustry.FIndustryId))
                {
                    return Conflict("行業已存在");
                }
                else
                {
                    throw;
                }
            }

            industryDTO.FIndustryId = newIndustry.FIndustryId;

            return CreatedAtAction(nameof(GetTSopIndustry), new { id = newIndustry.FIndustryId }, industryDTO);
        }

        // DELETE: api/TSopIndustries/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSopIndustry(int id)
        {
            var industry = await _context.TSopIndustries.FindAsync(id);
            if (industry == null)
            {
                return NotFound("行業不存在");
            }

            _context.TSopIndustries.Remove(industry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查行業是否存在
        private bool TSopIndustryExists(int id)
        {
            return _context.TSopIndustries.Any(e => e.FIndustryId == id);
        }
    }
}
