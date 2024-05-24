using Microsoft.AspNetCore.Mvc;

namespace prjAjaxDemo.Controllers
{
    public class HomeworkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Check()
        {
            return View();
        }
        public IActionResult Spots()
        {
            return View();
        }
        public IActionResult SpotsDetail()
        {
            return View();
        }
    }
}
