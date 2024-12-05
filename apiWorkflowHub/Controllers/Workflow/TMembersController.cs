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
    [EnableCors("ALL")]//一定要加入[EnableCors("ALL")]
    [ApiController]
    public class TMembersController : ControllerBase
    {
        private readonly SOPMarketContext _context;

        // 建構函式，將資料庫上下文注入到控制器中，這樣可以在每個 API 方法中操作資料庫
        public TMembersController(SOPMarketContext context)
        {
            _context = context;
        }

        // GET: api/TMembers
        // 這個方法返回所有會員資料，並且使用 DTO 進行數據傳輸
        [HttpGet]
        public async Task<IEnumerable<TMemberDTO>> GetTMembers()
        {
            // 將 TMember 實體列表轉換成 TMemberDTO 列表，僅返回必要的欄位
            return _context.TMembers.Select(member => new TMemberDTO
            {
                FName = member.FName,          // 會員姓名
                FEmail = member.FEmail         // 會員電子郵件
            });
            // 注意：這裡沒有使用 async/await，因為查詢已經立即執行，不需要等待非同步結果
        }

        // GET: api/TMembers/5
        // 這個方法根據會員 ID 查詢單一會員的資料
        [HttpGet("{id}")]
        public async Task<TMemberDTO> GetTMember(int id)
        {
            // 使用 FindAsync 根據會員 ID 查詢資料庫中的會員資料
            var tMember = await _context.TMembers.FindAsync(id);

            // 如果查詢結果為 null，則表示沒有找到對應的會員，返回 null
            if (tMember == null)
            {
                return null;
            }

            // 如果找到對應會員，則將其轉換成 TMemberDTO 並返回
            return new TMemberDTO
            {
                FName = tMember.FName,          // 會員姓名
                FEmail = tMember.FEmail         // 會員電子郵件
            };
        }

        // PUT: api/TMembers/5
        // 更新會員資料，使用 PUT 方法來修改現有的會員
        [HttpPut("{id}")]
        public async Task<string> PutTMember(int id, TMemberDTO tMemberDTO)
        {
            // 如果 URL 中的 ID 和 DTO 中的會員 ID 不一致，返回錯誤訊息
            //if (id != tMemberDTO.FMemberId)
            //{
            //    return "修改會員紀錄失敗";  // 傳回失敗訊息
            //}

            // 根據 ID 從資料庫中查詢對應的會員資料
            var tMember = await _context.TMembers.FindAsync(id);
            if (tMember == null)
            {
                return "會員不存在";  // 如果找不到會員，返回錯誤訊息
            }

            // 更新實體的屬性，將 DTO 中的值映射回實體
            tMember.FName = tMemberDTO.FName;
            tMember.FEmail = tMemberDTO.FEmail;

            // 標記這個實體狀態為已修改，通知 EF 進行更新
            _context.Entry(tMember).State = EntityState.Modified;

            try
            {
                // 保存變更到資料庫
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 如果更新過程中出現並發問題，檢查會員是否仍然存在
                if (!TMemberExists(id))
                {
                    return "修改會員紀錄失敗";  // 如果會員已經被刪除，返回錯誤訊息
                }
                else
                {
                    throw;  // 如果是其他錯誤，則拋出異常
                }
            }

            return "修改訊息成功";  // 更新成功後返回成功訊息
        }

        // POST: api/TMembers
        // 新增一筆會員資料
        [HttpPost]
        public async Task<TMemberDTO> PostTMember(TMemberDTO tMemberDTO)
        {
            // 創建一個新的 TMember 實體，並將 DTO 中的數據映射到實體中
            var tMember = new TMember
            {
                FName = tMemberDTO.FName,    // 會員姓名
                FEmail = tMemberDTO.FEmail   // 會員電子郵件
            };

            // 將新會員加入到資料庫上下文中，準備保存到資料庫
            _context.TMembers.Add(tMember);
            await _context.SaveChangesAsync();  // 保存變更到資料庫

            // 將資料庫生成的 ID 更新到 DTO 中，返回給前端
            //tMemberDTO.FMemberId = tMember.FMemberId;
            return tMemberDTO;
        }

        // DELETE: api/TMembers/5
        // 根據會員 ID 刪除一筆會員資料
        [HttpDelete("{id}")]
        public async Task<string> DeleteTMember(int id)
        {
            // 查詢資料庫中是否存在對應 ID 的會員
            var tMember = await _context.TMembers.FindAsync(id);
            if (tMember == null)
            {
                return "刪除會員紀錄失敗";  // 如果找不到會員，返回錯誤訊息
            }

            // 從資料庫中移除該會員
            _context.TMembers.Remove(tMember);
            await _context.SaveChangesAsync();  // 保存變更到資料庫

            return "刪除會員紀錄成功";  // 刪除成功後返回成功訊息
        }

        // 檢查會員是否存在於資料庫中，根據會員 ID 查詢
        private bool TMemberExists(int id)
        {
            return _context.TMembers.Any(e => e.FMemberId == id);
        }
    }
}
