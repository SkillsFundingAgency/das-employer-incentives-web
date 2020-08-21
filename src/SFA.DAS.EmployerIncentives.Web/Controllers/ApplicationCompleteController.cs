using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;
using System;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}")]
    public class ApplicationCompleteController : Controller
    {
        public const string ApplicationCompleteRoute = "application-complete";
        private readonly ExternalLinksConfiguration _configuration;

        public ApplicationCompleteController(IOptions<ExternalLinksConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{ApplicationCompleteRoute}/{applicationId}")]
        public IActionResult Confirmation()
        {
            var model = new ConfirmationViewModel(_configuration.ManageApprenticeshipSiteUrl);
            return View(model);
        }
    }
}
