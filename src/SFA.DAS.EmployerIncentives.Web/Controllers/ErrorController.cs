using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [AllowAnonymous()]
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("403")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("500")]
        public IActionResult ApplicationError()
        {
            return View();
        }
    }
}
