using Microsoft.AspNetCore.Mvc;

namespace caa_mis.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
