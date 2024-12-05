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
    public class TCompanySizesController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TCompanySizesController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TCompanySizes
        [HttpGet]
        public async Task<IEnumerable<TCompanySizeDTO>> GetTCompanySizes()
        {
            // 將所有 TCompanySize 轉換為 DTO 格式
            return await _context.TCompanySizes
                .OrderBy(size => size.FSizeSort) // 根據 FSizeSort 進行排序
                .Select(size => new TCompanySizeDTO
                {
                    FCompanySize = size.FCompanySize,
                    FSizeSort = size.FSizeSort
                })
                .ToListAsync();
        }

        // GET: api/TCompanySizes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TCompanySizeDTO>> GetTCompanySize(string id)
        {
            // 查詢特定的公司規模
            var companySize = await _context.TCompanySizes.FindAsync(id);

            if (companySize == null)
            {
                return NotFound("公司規模不存在");
            }

            // 返回 DTO 格式的資料
            var companySizeDTO = new TCompanySizeDTO
            {
                FCompanySize = companySize.FCompanySize,
                FSizeSort = companySize.FSizeSort
            };

            return companySizeDTO;
        }

        // PUT: api/TCompanySizes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTCompanySize(string id, TCompanySizeDTO companySizeDTO)
        {
            if (id != companySizeDTO.FCompanySize)
            {
                return BadRequest("公司規模 ID 不匹配");
            }

            var existingCompanySize = await _context.TCompanySizes.FindAsync(id);
            if (existingCompanySize == null)
            {
                return NotFound("公司規模不存在");
            }

            // 更新資料
            existingCompanySize.FSizeSort = companySizeDTO.FSizeSort;

            _context.Entry(existingCompanySize).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TCompanySizeExists(id))
                {
                    return NotFound("公司規模已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TCompanySizes
        [HttpPost]
        public async Task<ActionResult<TCompanySizeDTO>> PostTCompanySize(TCompanySizeDTO companySizeDTO)
        {
            var newCompanySize = new TCompanySize
            {
                FCompanySize = companySizeDTO.FCompanySize,
                FSizeSort = companySizeDTO.FSizeSort
            };

            _context.TCompanySizes.Add(newCompanySize);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TCompanySizeExists(companySizeDTO.FCompanySize))
                {
                    return Conflict("公司規模已存在");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetTCompanySize), new { id = newCompanySize.FCompanySize }, companySizeDTO);
        }

        // DELETE: api/TCompanySizes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTCompanySize(string id)
        {
            var companySize = await _context.TCompanySizes.FindAsync(id);
            if (companySize == null)
            {
                return NotFound("公司規模不存在");
            }

            _context.TCompanySizes.Remove(companySize);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查公司規模是否存在
        private bool TCompanySizeExists(string id)
        {
            return _context.TCompanySizes.Any(e => e.FCompanySize == id);
        }
    }
}
