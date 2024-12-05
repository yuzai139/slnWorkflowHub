using Microsoft.AspNetCore.Mvc;

namespace prjWorkflowHubAdmin.Controllers.Workflow
{
    public class WorkflowPublisherController : Controller
    {
        public IActionResult WorkflowPublisherIndex()
        {
            return View();
        }
		public IActionResult WorkflowPublisherCanvas()
		{
			return View();
		}
	}
}
