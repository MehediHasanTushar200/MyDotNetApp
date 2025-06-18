using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;

namespace hamko.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("Dashboard");
        }
    }

}
