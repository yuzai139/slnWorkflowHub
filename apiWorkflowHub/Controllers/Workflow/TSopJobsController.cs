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
    public class TSopJobsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TSopJobsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TSopJobs
        [HttpGet]
        public async Task<IEnumerable<TSopJobDTO>> GetTSopJobs()
        {
            return await _context.TSopJobs
                .OrderBy(job => job.FJobSort)
                .Select(job => new TSopJobDTO
                {
                    FJobId = job.FJobId,
                    FJob = job.FJob,
                    FJobClassId = job.FJobClassId,
                    FJobSort = job.FJobSort
                })
                .ToListAsync();
        }

        // GET: api/TSopJobs/{jobClassId}
        [HttpGet("{jobClassId}")]
        public async Task<ActionResult<IEnumerable<TSopJobDTO>>> GetTSopJobsByJobClass(int jobClassId)
        {
            var jobs = await _context.TSopJobs
                .Where(job => job.FJobClassId == jobClassId)
                .Select(job => new TSopJobDTO
                {
                    FJobId = job.FJobId,
                    FJob = job.FJob,
                    FJobClassId = job.FJobClassId,
                    FJobSort = job.FJobSort
                })
                .ToListAsync();

            if (!jobs.Any())
            {
                return NotFound("沒有找到符合條件的職業");
            }

            return Ok(jobs);
        }

        // PUT: api/TSopJobs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSopJob(int id, TSopJobDTO jobDTO)
        {
            if (id != jobDTO.FJobId)
            {
                return BadRequest("職業 ID 不匹配");
            }

            var existingJob = await _context.TSopJobs.FindAsync(id);
            if (existingJob == null)
            {
                return NotFound("職業不存在");
            }

            existingJob.FJob = jobDTO.FJob;
            existingJob.FJobClassId = jobDTO.FJobClassId;
            existingJob.FJobSort = jobDTO.FJobSort;

            _context.Entry(existingJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSopJobExists(id))
                {
                    return NotFound("職業已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TSopJobs
        [HttpPost]
        public async Task<ActionResult<TSopJobDTO>> PostTSopJob(TSopJobDTO jobDTO)
        {
            var newJob = new TSopJob
            {
                FJob = jobDTO.FJob,
                FJobClassId = jobDTO.FJobClassId,
                FJobSort = jobDTO.FJobSort
            };

            _context.TSopJobs.Add(newJob);
            await _context.SaveChangesAsync();

            jobDTO.FJobId = newJob.FJobId;

            return CreatedAtAction(nameof(GetTSopJobsByJobClass), new { jobClassId = newJob.FJobClassId }, jobDTO);
        }

        // DELETE: api/TSopJobs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSopJob(int id)
        {
            var job = await _context.TSopJobs.FindAsync(id);
            if (job == null)
            {
                return NotFound("職業不存在");
            }

            _context.TSopJobs.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查職業是否存在
        private bool TSopJobExists(int id)
        {
            return _context.TSopJobs.Any(e => e.FJobId == id);
        }
    }
}
