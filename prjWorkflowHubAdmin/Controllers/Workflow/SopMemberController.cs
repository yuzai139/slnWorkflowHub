using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace prjWorkflowHubAdmin.Controllers.Workflow
{
    [EnableCors("All")] // 確保允許 CORS
    public class SopMemberController : Controller
    {
        public IActionResult SopMemberList()
        {
            return View();
        }

        public IActionResult SopMemberEdit(int sopId)
        {
            // 確保 sopId 被傳遞並顯示到頁面中
            ViewData["SopId"] = sopId;
            return View();
        }
    }
}
