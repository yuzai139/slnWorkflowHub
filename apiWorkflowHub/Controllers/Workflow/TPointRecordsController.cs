using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using Microsoft.AspNetCore.Cors;
using apiWorkflowHub.DTO.Workflow;

namespace apiWorkflowHub.Controllers.Workflow
{
    [Route("api/[controller]")]
    [EnableCors("ALL")]
    [ApiController]
    public class TPointRecordsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TPointRecordsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TPointRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TPointRecordDTO>>> GetTPointRecords()
        {
            // 將資料轉換為 DTO 格式
            var records = await _context.TPointRecords
                .Select(record => new TPointRecordDTO
                {
                    FPointRecordId = record.FPointRecordId,
                    FMemberId = record.FMemberId,
                    FPointRecord = record.FPointRecord,
                    FExplanation = record.FExplanation,
                    FRecordTime = record.FRecordTime
                }).ToListAsync();

            return Ok(records);
        }

        // GET: api/TPointRecords/5
        [HttpGet("{memberId}")]
        public async Task<ActionResult<IEnumerable<TPointRecordDTO>>> GetTPointRecordsByMember(int memberId)
        {
            var records = await _context.TPointRecords
                .Where(record => record.FMemberId == memberId)
                .Select(record => new TPointRecordDTO
                {
                    FPointRecordId = record.FPointRecordId,
                    FMemberId = record.FMemberId,
                    FPointRecord = record.FPointRecord,
                    FExplanation = record.FExplanation,
                    FRecordTime = record.FRecordTime
                }).ToListAsync();

            if (records == null || !records.Any())
            {
                return NotFound("此會員沒有點數紀錄");
            }

            return Ok(records);
        }

        // PUT: api/TPointRecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTPointRecord(int id, TPointRecordDTO tPointRecordDto)
        {
            if (id != tPointRecordDto.FPointRecordId)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            var record = await _context.TPointRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound("Record not found.");
            }

            // 更新資料庫中的紀錄
            record.FMemberId = tPointRecordDto.FMemberId;
            record.FPointRecord = tPointRecordDto.FPointRecord;
            record.FExplanation = tPointRecordDto.FExplanation;
            record.FRecordTime = tPointRecordDto.FRecordTime;

            _context.Entry(record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TPointRecordExists(id))
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

        // POST: api/TPointRecords
        [HttpPost]
        public async Task<ActionResult<TPointRecordDTO>> PostTPointRecord(TPointRecordDTO tPointRecordDto)
        {
            var newRecord = new TPointRecord
            {
                FMemberId = tPointRecordDto.FMemberId,
                FPointRecord = tPointRecordDto.FPointRecord,
                FExplanation = tPointRecordDto.FExplanation,
                FRecordTime = tPointRecordDto.FRecordTime
            };

            _context.TPointRecords.Add(newRecord);
            await _context.SaveChangesAsync();

            tPointRecordDto.FPointRecordId = newRecord.FPointRecordId; // 設置 ID 回傳至 DTO

            return CreatedAtAction(nameof(GetTPointRecordsByMember), new { memberId = newRecord.FMemberId }, tPointRecordDto);
        }

        // DELETE: api/TPointRecords/5
        [HttpDelete("{pointRecordId}")]
        public async Task<IActionResult> DeleteTPointRecord(int pointRecordId)
        {
            var record = await _context.TPointRecords.FindAsync(pointRecordId);
            if (record == null)
            {
                return NotFound("沒有找到紀錄");
            }

            _context.TPointRecords.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(new { 刪除點數紀錄ID = pointRecordId});
        }

        private bool TPointRecordExists(int id)
        {
            return _context.TPointRecords.Any(e => e.FPointRecordId == id);
        }
    }
}
