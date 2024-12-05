using Microsoft.AspNetCore.Mvc;

namespace prjWorkflowHubAdmin.Controllers.CartAndOrder
{
    public class CartController : Controller
    {
        public IActionResult CartIndex()
        {
            return View();
        }
    }
}
