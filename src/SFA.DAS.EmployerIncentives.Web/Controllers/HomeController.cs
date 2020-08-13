using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}")]
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Home(string accountId)
        {
            return RedirectToAction("Default", "apply", new { accountId});
        }
    }
}