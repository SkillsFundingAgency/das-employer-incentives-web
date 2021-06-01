using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
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
        [Route("{accountId}/system-update")]
        public async Task<IActionResult> SystemUpdate(string accountId)
        {
            return View(new SystemUpdateModel(accountId, _configuration.ManageApprenticeshipSiteUrl));
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously