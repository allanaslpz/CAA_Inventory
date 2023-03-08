using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caa_mis.Controllers
{
    [Authorize(Roles = "Admin, Supervisor")]
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
