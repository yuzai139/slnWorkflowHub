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
    [EnableCors("ALL")] // 確保允許 CORS
    [ApiController]
    public class TMmbSopsController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        public TMmbSopsController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TMmbSops
        [HttpGet]
        public async Task<IEnumerable<TMemberSopDTO>> GetTSops()
        {
            // 返回所有 SOP 資料並轉換為 DTO
            return _context.TSops.Select(sop => new TMemberSopDTO
            {
                FSopid = sop.FSopid,
                FMemberId = sop.FMemberId,
                FSopType = sop.FSopType,
                FSopName = sop.FSopName,
                FSopDescription = sop.FSopDescription,
                FSopFlowImagePath = sop.FSopFlowImagePath,
                FJobItemId = sop.FJobItemId,
                FIndustryId = sop.FIndustryId,
                FBusiness = sop.FBusiness,
                FCustomer = sop.FCustomer,
                FCompanySize = sop.FCompanySize,
                FDepartment = sop.FDepartment,
                FShareUrl = sop.FShareUrl,
                FEditTime = sop.FEditTime,
                FSharePermission = sop.FSharePermission,
                FFileStatus = sop.FFileStatus
            }).ToList();
        }

        // GET: api/TMmbSops/id
        [HttpGet("{id}")]
        public async Task<ActionResult<TMemberSopDTO>> GetTSop(int id)
        {
            // 根據 ID 查詢 SOP
            var tSop = await _context.TSops.FindAsync(id);

            // 如果 SOP 不存在，返回 404 錯誤
            if (tSop == null)
            {
                return NotFound("SOP 不存在");
            }

            // 返回查詢到的 SOP DTO
            var sopDTO = new TMemberSopDTO
            {
                FSopid = tSop.FSopid,
                FMemberId = tSop.FMemberId,
                FSopType = tSop.FSopType,
                FSopName = tSop.FSopName,
                FSopDescription = tSop.FSopDescription,
                FSopFlowImagePath = tSop.FSopFlowImagePath,
                FJobItemId = tSop.FJobItemId,
                FIndustryId = tSop.FIndustryId,
                FBusiness = tSop.FBusiness,
                FCustomer = tSop.FCustomer,
                FCompanySize = tSop.FCompanySize,
                FDepartment = tSop.FDepartment,
                FShareUrl = tSop.FShareUrl,
                FEditTime = tSop.FEditTime,
                FSharePermission = tSop.FSharePermission,
                FFileStatus = tSop.FFileStatus
            };

            return sopDTO;
        }

		// PUT: api/TMmbSops/5
		[HttpPut("{Sopid}")]
        public async Task<IActionResult> PutTSop(int Sopid, [FromBody] TMemberSopDTO tSopDTO)
        {
			// 檢查 ModelState 是否有效
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState); // 這樣可以返回具體的錯誤信息
			}


			// 檢查是否為正確的 SOP ID
			if (Sopid != tSopDTO.FSopid)
            {
                return BadRequest("SOP ID 不匹配");
            }

            // 查詢現有的 SOP
            var tSop = await _context.TSops.FindAsync(Sopid);
            if (tSop == null)
            {
                return NotFound("SOP 不存在");
            }

            // 更新實體資料
            tSop.FMemberId = tSopDTO.FMemberId;
            tSop.FSopType = tSopDTO.FSopType;
            tSop.FSopName = tSopDTO.FSopName;
            tSop.FSopDescription = tSopDTO.FSopDescription;
            tSop.FSopFlowImagePath = tSopDTO.FSopFlowImagePath;
            tSop.FJobItemId = tSopDTO.FJobItemId;
            tSop.FIndustryId = tSopDTO.FIndustryId;
            tSop.FBusiness = tSopDTO.FBusiness;
            tSop.FCustomer = tSopDTO.FCustomer;
            tSop.FCompanySize = tSopDTO.FCompanySize;
            tSop.FDepartment = tSopDTO.FDepartment;
            tSop.FShareUrl = tSopDTO.FShareUrl;
            tSop.FEditTime = tSopDTO.FEditTime;
            tSop.FSharePermission = tSopDTO.FSharePermission;
            tSop.FFileStatus = tSopDTO.FFileStatus;

            // 標記狀態為已修改
            _context.Entry(tSop).State = EntityState.Modified;

            try
            {
                // 保存變更
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 檢查 SOP 是否仍存在
                if (!TSopExists(Sopid))
                {
                    return NotFound("SOP 已不存在");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TSops
        [HttpPost]
        public async Task<ActionResult<TMemberSopDTO>> PostTSop(TMemberSopDTO tSopDTO)
        {
            // 創建一個新的 SOP 實體
            var tSop = new TSop
            {
                FMemberId = tSopDTO.FMemberId,
                FSopType = tSopDTO.FSopType,
                FSopName = tSopDTO.FSopName,
                FSopDescription = tSopDTO.FSopDescription,
                FSopFlowImagePath = tSopDTO.FSopFlowImagePath,
                FJobItemId = tSopDTO.FJobItemId,
                FIndustryId = tSopDTO.FIndustryId,
                FBusiness = tSopDTO.FBusiness,
                FCustomer = tSopDTO.FCustomer,
                FCompanySize = tSopDTO.FCompanySize,
                FDepartment = tSopDTO.FDepartment,
                FShareUrl = tSopDTO.FShareUrl,
                FEditTime = tSopDTO.FEditTime,
                FSharePermission = tSopDTO.FSharePermission,
                FFileStatus = tSopDTO.FFileStatus
            };

            // 將 SOP 加入資料庫
            _context.TSops.Add(tSop);
            await _context.SaveChangesAsync();

            // 更新 DTO 的 ID 並返回
            tSopDTO.FSopid = tSop.FSopid;

            return CreatedAtAction(nameof(GetTSop), new { id = tSop.FSopid }, tSopDTO);
        }

        // DELETE: api/TSops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSop(int id)
        {
            // 查詢 SOP
            var tSop = await _context.TSops.FindAsync(id);
            if (tSop == null)
            {
                return NotFound("SOP 不存在");
            }

            // 從資料庫中移除該 SOP
            _context.TSops.Remove(tSop);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查 SOP 是否存在
        private bool TSopExists(int id)
        {
            return _context.TSops.Any(e => e.FSopid == id);
        }
    }
}
