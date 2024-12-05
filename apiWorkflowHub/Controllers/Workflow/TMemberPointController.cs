using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Workflow;
using System.Net;
using Microsoft.AspNetCore.Cors;

namespace apiWorkflowHub.Controllers.Workflow
{
    [Route("api/[controller]")]
    [EnableCors("ALL")]
    [ApiController]
    public class TMemberPointController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TMemberPointController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TMemberPoint
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TMemberPointDTO>>> GetTMembers()
        {
            return await _context.TMembers
                .Select(m => new TMemberPointDTO
                {
                    FMemberId = m.FMemberId,
                    FName = m.FName,
                    FMemberPoints = m.FMemberPoints,
                    FMemberShip = m.FMemberShip
                })
                .ToListAsync();
        }

        // GET: api/TMemberPoint/5 //取得會員點數
        [HttpGet("{memberid}")]
        public async Task<ActionResult<TMemberPointDTO>> GetTMember(int memberid)
        {
            var tMemberPointDTO = await _context.TMembers
                .Where(m => m.FMemberId == memberid)
                .Select(m => new TMemberPointDTO
                {
                    FMemberId = m.FMemberId,
                    FName = m.FName,
                    FMemberPoints = m.FMemberPoints,
                    FMemberShip = m.FMemberShip
                })
                .FirstOrDefaultAsync();

            if (tMemberPointDTO == null)
            {
                return NotFound();
            }

            return tMemberPointDTO;
        }


        // PUT: api/TMemberPoint/add/{memberId}/{addPoint}  // 增加點數並記錄操作
        [HttpPut("add/{memberId}/{addPoint}")]
        public async Task<IActionResult> AddPoints(int memberId, int addPoint)
        {
            // 查找會員資料
            var tMember = await _context.TMembers.FindAsync(memberId);
            if (tMember == null)
            {
                return NotFound("找不到指定的會員。");
            }

            // 檢查增加點數是否合法
            if (addPoint <= 0)
            {
                return BadRequest("增加的點數必須大於零。");
            }

            // 增加會員點數，處理 null 值
            tMember.FMemberPoints = (tMember.FMemberPoints ?? 0) + addPoint;
            _context.Entry(tMember).State = EntityState.Modified;

            // 獲取台灣時間並格式化為字串
            string taiwanTimeString = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");

            // 建立新的點數紀錄
            TPointRecord newPointRecord = new TPointRecord
            {
                FMemberId = memberId,
                FPointRecord = $"增加 {addPoint} 點",
                FExplanation = "發佈新的工作流程獎勵點數",
                FRecordTime = taiwanTimeString
            };

            // 添加點數紀錄至資料庫
            _context.TPointRecords.Add(newPointRecord);

            try
            {
                // 保存變更
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TMemberExists(memberId))
                {
                    return NotFound("找不到指定的會員。");
                }
                else
                {
                    throw;
                }
            }

            // 回傳成功訊息
            return Ok(new
            {
                Message = "點數增加成功",
                會員Id = tMember.FMemberId,
                會員名稱 = tMember.FName,
                會員更新後的點數 = tMember.FMemberPoints,
                點數紀錄Id = newPointRecord.FPointRecordId,
            });
        }


        // PUT: api/TMemberPoint/reduce/{memberId}/{reducePoint}  // 減少點數
        [HttpPut("reduce/{memberId}/{reducePoint}")]
        public async Task<IActionResult> ReducePoints(int memberId, int reducePoint)
        {
            var tMember = await _context.TMembers.FindAsync(memberId);
            if (tMember == null)
            {
                return NotFound("找不到指定的會員。");
            }

            // 檢查減少的點數是否合法
            if (reducePoint <= 0)
            {
                return BadRequest("減少的點數必須大於零。");
            }

            // 確保點數不低於零
            if ((tMember.FMemberPoints ?? 0) < reducePoint)
            {
                return BadRequest("減少的點數超過會員目前的點數。");
            }

            tMember.FMemberPoints -= reducePoint;

            _context.Entry(tMember).State = EntityState.Modified;


            // 獲取台灣時間並格式化為字串
            string taiwanTimeString = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");

            // 建立新的點數紀錄
            TPointRecord newPointRecord = new TPointRecord
            {
                FMemberId = memberId,
                FPointRecord = $"扣除 {reducePoint} 點",
                FExplanation = "購買商品花費",
                FRecordTime = taiwanTimeString
            };

            // 添加點數紀錄至資料庫
            _context.TPointRecords.Add(newPointRecord);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TMemberExists(memberId))
                {
                    return NotFound("找不到指定的會員。");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Message = "點數減少成功", MemberId = tMember.FMemberId, MemberName = tMember.FName, NewPoints = tMember.FMemberPoints });
        }



        // POST: api/TMemberPoint/AddPointsByOrderId/{orderId}  //用OrderId批次給發布者增加點數
        [HttpPost("AddPointsByOrderId/{orderId}")]
        public async Task<IActionResult> AddPointsByOrder(int orderId)
        {
            try
            {
                // 1. 查詢指定訂單的所有產品 (取得每個 SopId)
                var orderProducts = await _context.TSopProducts
                    .Where(p => p.FOrderId == orderId)
                    .Select(p => p.FSopid)
                    .ToListAsync();

                if (!orderProducts.Any())
                {
                    return NotFound("找不到與此訂單相關的產品。");
                }

                // 2. 查詢每個 SopId 的商家資料 (MemberId) 和點數 (FSalePoints)
                var sopDetails = await _context.TSops
                    .Where(s => orderProducts.Contains(s.FSopid))
                    .Select(s => new TSopPricePointDTO
                    {
                        FSopid = s.FSopid,
                        FMemberId = s.FMemberId,
                        FSalePoints = s.FSalePoints
                    })
                    .ToListAsync();

                if (!sopDetails.Any())
                {
                    return NotFound("找不到對應的 SOP 資料。");
                }

                // 3. 計算每個商家 (MemberId) 的總點數（打 8 折）
                var pointsByMember = new Dictionary<int, decimal>();

                foreach (var sop in sopDetails)
                {
                    if (sop.FMemberId == null || sop.FSalePoints == null) continue;

                    // 計算應增加的點數（打 8 折）
                    var discountedPoints = sop.FSalePoints.Value * 0.8m;

                    if (pointsByMember.ContainsKey(sop.FMemberId.Value))
                    {
                        pointsByMember[sop.FMemberId.Value] += discountedPoints;
                    }
                    else
                    {
                        pointsByMember[sop.FMemberId.Value] = discountedPoints;
                    }
                }

                // 4. 批量查詢所有需要更新的會員
                var memberIds = pointsByMember.Keys.ToList();
                var members = await _context.TMembers
                    .Where(m => memberIds.Contains(m.FMemberId))
                    .ToListAsync();

                // 5. 更新每個會員的點數，並記錄結果和點數紀錄
                var updateResults = new List<object>();
                var pointRecords = new List<TPointRecord>();

                foreach (var member in members)
                {
                    if (pointsByMember.TryGetValue(member.FMemberId, out var pointsToAdd))
                    {
                        member.FMemberPoints = (member.FMemberPoints ?? 0) + (int)pointsToAdd;

                        // 新增點數紀錄
                        pointRecords.Add(new TPointRecord
                        {
                            FMemberId = member.FMemberId,
                            FPointRecord = $"增加 {(int)pointsToAdd} 點",
                            FExplanation = "銷售工作流程所得 ( 80 % 分潤 )",
                            FRecordTime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") // 台灣時間
                        });

                        // 記錄更新的結果
                        updateResults.Add(new
                        {
                            會員Id = member.FMemberId,
                            增加點數 = (int)pointsToAdd,
                            增加後的點數 = member.FMemberPoints
                        });
                    }
                }

                // 批量保存點數紀錄
                _context.TPointRecords.AddRange(pointRecords);

                await _context.SaveChangesAsync(); // 批量保存更改

                // 回傳每個會員的更新結果
                return Ok(new { Message = "會員點數更新成功！", 會員點數更新名單 = updateResults });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"資料庫錯誤：{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"發生錯誤：{ex.Message}");
            }
        }


        // POST: api/TMemberPoint/AddPointsBySopIds  //用sopId陣列批次給發布者增加點數
        [HttpPost("AddPointsBySopIds")]
        public async Task<IActionResult> AddPointsBySopIds([FromBody] List<int> sopIds)
        {
            if (sopIds == null || !sopIds.Any())
            {
                return BadRequest("SopId 陣列為空或無效。");
            }

            try
            {
                // 1. 查詢每個 SopId 的商家資料 (MemberId) 和點數 (FSalePoints)
                var sopDetails = await _context.TSops
                    .Where(s => sopIds.Contains(s.FSopid))
                    .Select(s => new TSopPricePointDTO
                    {
                        FSopid = s.FSopid,
                        FMemberId = s.FMemberId,
                        FSalePoints = s.FSalePoints
                    })
                    .ToListAsync();

                if (!sopDetails.Any())
                {
                    return NotFound("找不到對應的 SOP 資料。");
                }

                // 2. 計算每個商家 (MemberId) 的總點數（打 8 折）
                var pointsByMember = new Dictionary<int, decimal>();
                foreach (var sop in sopDetails)
                {
                    if (sop.FMemberId == null || sop.FSalePoints == null) continue;

                    // 計算應增加的點數（打 8 折）
                    var discountedPoints = sop.FSalePoints.Value * 0.8m;

                    if (pointsByMember.ContainsKey(sop.FMemberId.Value))
                    {
                        pointsByMember[sop.FMemberId.Value] += discountedPoints;
                    }
                    else
                    {
                        pointsByMember[sop.FMemberId.Value] = discountedPoints;
                    }
                }

                // 3. 批量查詢所有需要更新的會員
                var memberIds = pointsByMember.Keys.ToList();
                var members = await _context.TMembers
                    .Where(m => memberIds.Contains(m.FMemberId))
                    .ToListAsync();

                // 4. 更新每個會員的點數，並記錄結果和點數紀錄
                var updateResults = new List<object>();
                var pointRecords = new List<TPointRecord>();

                foreach (var member in members)
                {
                    if (pointsByMember.TryGetValue(member.FMemberId, out var pointsToAdd))
                    {
                        member.FMemberPoints = (member.FMemberPoints ?? 0) + (int)pointsToAdd;

                        // 新增點數紀錄
                        pointRecords.Add(new TPointRecord
                        {
                            FMemberId = member.FMemberId,
                            FPointRecord = $"增加 {(int)pointsToAdd} 點",
                            FExplanation = "銷售工作流程所得",
                            FRecordTime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") // 台灣時間
                        });

                        // 記錄更新的結果
                        updateResults.Add(new
                        {
                            會員Id = member.FMemberId,
                            增加點數 = (int)pointsToAdd,
                            增加後的點數 = member.FMemberPoints
                        });
                    }
                }

                // 批量保存點數紀錄
                _context.TPointRecords.AddRange(pointRecords);

                await _context.SaveChangesAsync(); // 批量保存更改

                // 回傳每個會員的更新結果
                return Ok(new { Message = "會員點數更新成功！", 會員點數更新名單 = updateResults });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"資料庫錯誤：{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"發生錯誤：{ex.Message}");
            }
        }



        // POST: api/TMemberPoint
        [HttpPost]
        public async Task<ActionResult<TMemberPointDTO>> PostTMember(TMemberPointDTO tMemberDTO)
        {
            var tMember = new TMember
            {
                FMemberId = tMemberDTO.FMemberId,
                FName = tMemberDTO.FName,
                FMemberPoints = tMemberDTO.FMemberPoints,
                FMemberShip = tMemberDTO.FMemberShip
            };

            _context.TMembers.Add(tMember);
            await _context.SaveChangesAsync();

            // 回傳新增的 DTO 資料
            return CreatedAtAction("GetTMember", new { id = tMember.FMemberId }, tMemberDTO);
        }

        // DELETE: api/TMemberPoint/5
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
