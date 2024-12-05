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
    public class FTMembersController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public FTMembersController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TMembers
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TMember>>> GetTMembers()
        //{
        //    return await _context.TMembers.ToListAsync();
        //}

        // GET: api/TMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DTMember>> GetTMember(int id)
        {
            var tMember = await _context.TMembers.FindAsync(id);

            if (tMember == null)
            {
                return NotFound();
            }

            return DTMember.FromEntity(tMember);
        }

        // PUT: api/TMembers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTMember(int id, TMember tMember)
        //{
        //    if (id != tMember.FMemberId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(tMember).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TMemberExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/TMembers
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<TMember>> PostTMember(TMember tMember)
        //{
        //    _context.TMembers.Add(tMember);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTMember", new { id = tMember.FMemberId }, tMember);
        //}

        //// DELETE: api/TMembers/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTMember(int id)
        //{
        //    var tMember = await _context.TMembers.FindAsync(id);
        //    if (tMember == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TMembers.Remove(tMember);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool TMemberExists(int id)
        //{
        //    return _context.TMembers.Any(e => e.FMemberId == id);
        //}
    }
}
