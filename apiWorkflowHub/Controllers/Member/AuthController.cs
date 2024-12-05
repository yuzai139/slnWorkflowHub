using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Member;

namespace apiWorkflowHub.Controllers.Member
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public AuthController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/Auth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TMember>>> GetTMembers()
        {
            return await _context.TMembers.ToListAsync();
        }

        // GET: api/Auth/5
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

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<TMember>> Login([FromBody] UserDto userloginDTO)
        {
            try
            {
                var user = await _context.TMembers
                    .FirstOrDefaultAsync(u => u.FEmail == userloginDTO.fEmail);

                if (user == null)
                {
                    return NotFound(new { message = "查無此帳號，請先註冊" });
                }

                if (user.FPassword != userloginDTO.fPassword)
                {
                    return Unauthorized(new { message = "輸入密碼不正確" });
                }

                return Ok(new { message = "登入成功", memberId = user.FMemberId, memberName = user.FName });

            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = "伺服器發生錯誤", error = ex.Message });
            }


        }

        // PUT: api/Auth/5
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

        // POST: api/Auth
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TMember>> PostTMember(TMember tMember)
        {
            _context.TMembers.Add(tMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTMember", new { id = tMember.FMemberId }, tMember);
        }

        // DELETE: api/Auth/5
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


        //[HttpPost]
        //[Route("[action]")]
        //public async Task<ActionResult<TMember>> Login(UserDto userDto)
        //{
        //    return Ok();
        //    //_context.TMembers.Add(tMember);
        //    //await _context.SaveChangesAsync();

        //    //return CreatedAtAction("GetTMember", new { id = tMember.FMemberId }, tMember);
        //}

    }
}
