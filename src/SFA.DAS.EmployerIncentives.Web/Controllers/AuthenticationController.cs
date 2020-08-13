using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("authentication")]
    public class AuthenticationController : Controller
    {
        [Route("")]
        public IActionResult SignIn()
        {
            return new OkResult();
        }
    }
}