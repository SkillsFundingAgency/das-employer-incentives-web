using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.System;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{    
    public class SystemController : Controller
    {
        private readonly ExternalLinksConfiguration _configuration;

        public SystemController(IOptions<ExternalLinksConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        [HttpGet]
        [Route("{accountId}/applications-closed")]
        public IActionResult ApplicationsClosed(string accountId)
        {
            return View(new ApplicationsClosedModel(accountId, _configuration.ManageApprenticeshipSiteUrl));
        }

        [HttpGet]
        [Route("{accountId}/service-closed")]
        public IActionResult ServiceClosed(string accountId)
        {
            return View();
        }
    }
}