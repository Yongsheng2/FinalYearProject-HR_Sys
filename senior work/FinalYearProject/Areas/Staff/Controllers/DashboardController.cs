using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Areas.Staff.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
