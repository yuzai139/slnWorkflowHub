using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;

namespace apiWorkflowHub.Controllers.Member
{
    [Route("api/[controller]")]
    [ApiController]
    public class TMembersAPIController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TMembersAPIController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TMembersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TMember>>> GetTMembers()
        {
            return await _context.TMembers.ToListAsync();
        }

        // GET: api/TMembersAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TMember>> GetTMember(int id)
        {
            var tMember = await _context.TMembers.FindAsync(id);

            if (tMember == null)
            {
                return NotFound();
            }

            return tMember;
        }

        // PUT: api/TMembersAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTMember(int id, TMember tMember)
        {
            if (id != tMember.FMemberId)
            {
                return BadRequest();
            }

            _context.Entry(tMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TMemberExists(id))
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

        // POST: api/TMembersAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("login")]
        public async Task<ActionResult<TMember>> PostTMember(TMember tMember)
        {
            _context.TMembers.Add(tMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTMember", new { id = tMember.FMemberId }, tMember);
        }

        // DELETE: api/TMembersAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTMember(int id)
        {
            var tMember = await _context.TMembers.FindAsync(id);
            if (tMember == null)
            {
                return NotFound();
            }

            _context.TMembers.Remove(tMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TMemberExists(int id)
        {
            return _context.TMembers.Any(e => e.FMemberId == id);
        }
    }
}
