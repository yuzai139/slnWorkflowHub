using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace prjWorkflowHubAdmin.Controllers.Workflow
{
    [EnableCors("All")] // 確保允許 CORS
    public class SopPublisherController : Controller
    {
        //SopPublisher/SopPublisherEdit
        public IActionResult SopPublisherEdit(int sopId)
        {
            // 確保 sopId 被傳遞並顯示到頁面中
            ViewData["SopId"] = sopId;
            return View();
        }
    }
}
