using Microsoft.AspNetCore.Mvc;

namespace prjMVCDemo.Controllers
{
    public class TryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
