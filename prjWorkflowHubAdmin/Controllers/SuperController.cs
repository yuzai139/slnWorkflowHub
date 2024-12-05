using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjCoreMvcDemo.Models;

namespace prjCoreMvcDemo.Controllers
{
    public class SuperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_MEMBER))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login"
                }));
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
