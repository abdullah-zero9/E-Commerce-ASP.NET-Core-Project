using Microsoft.AspNetCore.Mvc;

namespace work_01_Session.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var s = HttpContext.Session.GetString("r53");
            ViewBag.msg = s;
            return View();
        }
    }
}
