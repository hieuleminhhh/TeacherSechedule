using Microsoft.AspNetCore.Mvc;

namespace WebSchedule.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
