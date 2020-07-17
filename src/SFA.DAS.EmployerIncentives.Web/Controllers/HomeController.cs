using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("help/cookie-settings")]
        public IActionResult CookieSettings()
        {
            return View();
        }

        [Route("help/cookie-details")]
        public IActionResult CookieDetails()
        {
            return View();
        }
    }
}