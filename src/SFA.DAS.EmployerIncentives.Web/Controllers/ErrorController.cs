using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Error;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [AllowAnonymous()]
    [Route("error")]
    public class ErrorController : Controller
    {
        private readonly ExternalLinksConfiguration _configuration;

        public ErrorController(IOptions<ExternalLinksConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        [Route("403")]
        public IActionResult AccessDenied()
        {
            return View(new ErrorViewModel(_configuration.ManageApprenticeshipSiteUrl));
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View(new ErrorViewModel(_configuration.ManageApprenticeshipSiteUrl));
        }

        [Route("500")]
        public IActionResult ApplicationError()
        {
            return View(new ErrorViewModel(_configuration.ManageApprenticeshipSiteUrl));
        }
    }
}
