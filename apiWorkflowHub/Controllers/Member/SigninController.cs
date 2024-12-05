using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Member;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using System.Security;

namespace apiWorkflowHub.Controllers.Member
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigninController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public SigninController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/Signin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TMember>>> GetTMembers()
        {
            return await _context.TMembers.ToListAsync();
        }

        // GET: api/Signin/5
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

        // PUT: api/Signin/5
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

        // POST: api/Signin
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<TMember>> PostTMember([FromBody] UserSigninDto userSigninDto)
        {

            TMember member = new TMember
            {
                FMemberId = userSigninDto.fMemberID,
                FName = userSigninDto.fName,
                FBirthday = userSigninDto.fBirthday,
                FPhone = userSigninDto.fPhone,
                FEmail = userSigninDto.fEmail,
                FAddress = userSigninDto.fAddress,
                FPassword = userSigninDto.fPassword,
                FPermissions = userSigninDto.fPermissions,
                FMemberPoints = userSigninDto.fMemberPoints,
                FMemberShip = userSigninDto.fMemberShip,
                FMailVerify = userSigninDto.fMailVerify,
                FSopexp = userSigninDto.fSopexp,
            };

            _context.TMembers.Add(member);
            await _context.SaveChangesAsync();

            return Ok(member);
            //return CreatedAtAction("GetTMember", new { id = userSigninDto.FMemberId }, tMember);
        }

        // DELETE: api/Signin/5
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
