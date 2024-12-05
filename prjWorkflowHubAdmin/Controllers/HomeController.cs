using Microsoft.AspNetCore.Mvc;
using prjCoreMvcDemo.Models;
using prjCoreMvcDemo.ViewModels;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.Models;
using System.Diagnostics;
using System.Text.Json;

namespace prjWorkflowHubAdmin.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_MEMBER))
                return RedirectToAction("Login");
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_MEMBER);
            TMember t = JsonSerializer.Deserialize<TMember>(json);
            ViewBag.WELCOME = t.FName;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CLoginViewModel vm)
        {
            TMember t = (new SOPMarketContext()).TMembers.FirstOrDefault(
                c => c.FEmail.Equals(vm.txtAccount));
            if (t != null)
            {
                if (t.FPassword.Equals(vm.txtPassword))
                {
                    string json = JsonSerializer.Serialize(t);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_MEMBER, json);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
