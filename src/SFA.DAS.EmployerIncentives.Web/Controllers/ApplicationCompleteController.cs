using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/application-complete/{applicationId}")]
    public class ApplicationCompleteController : Controller
    {
        private readonly ExternalLinksConfiguration _configuration;

        public ApplicationCompleteController(IOptions<ExternalLinksConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Confirmation()
        {
            var model = new ConfirmationViewModel(_configuration.ManageApprenticeshipSiteUrl);
            return View(model);
        }
    }
}
