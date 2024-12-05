using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using Microsoft.AspNetCore.Cors;
using apiWorkflowHub.Controllers.DTO.Forum;

namespace apiWorkflowHub.Controllers.Forum
{
    [Route("api/[controller]")]
    [EnableCors("ALL")] // 啟用 CORS
    [ApiController]
    public class TCategoriesController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TCategoriesController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTCategory>>> GetTCategories()
        {
            var categories = await _context.TCategories.ToListAsync();
            return categories.Select(DTCategory.FromEntity).ToList();
        }

        // GET: api/TCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TCategory>> GetTCategory(int id)
        {
            var tCategory = await _context.TCategories.FindAsync(id);

            if (tCategory == null)
            {
                return NotFound();
            }

            return tCategory;
        }

        // PUT: api/TCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTCategory(int id, TCategory tCategory)
        {
            if (id != tCategory.FCategoryNumber)
            {
                return BadRequest();
            }

            _context.Entry(tCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TCategoryExists(id))
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

        // POST: api/TCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TCategory>> PostTCategory(TCategory tCategory)
        {
            _context.TCategories.Add(tCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTCategory", new { id = tCategory.FCategoryNumber }, tCategory);
        }

        // DELETE: api/TCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTCategory(int id)
        {
            var tCategory = await _context.TCategories.FindAsync(id);
            if (tCategory == null)
            {
                return NotFound();
            }

            _context.TCategories.Remove(tCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TCategoryExists(int id)
        {
            return _context.TCategories.Any(e => e.FCategoryNumber == id);
        }
    }
}
