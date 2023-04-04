using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caa_mis.Controllers
{
    [Authorize(Roles = "Admin, Supervisor")]
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

            return View();
        }

    }
}
