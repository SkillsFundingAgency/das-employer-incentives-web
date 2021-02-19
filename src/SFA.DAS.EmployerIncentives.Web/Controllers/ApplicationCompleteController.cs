using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/application-complete/{applicationId}")]
    public class ApplicationCompleteController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly ExternalLinksConfiguration _configuration;

        public ApplicationCompleteController(ILegalEntitiesService legalEntitiesService,
                                             IApplicationService applicationService,
                                             IOptions<ExternalLinksConfiguration> configuration)
            : base(legalEntitiesService)
        {
            _applicationService = applicationService;
            _configuration = configuration.Value;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Confirmation(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            var model = new ConfirmationViewModel(accountId, application.AccountLegalEntityId, legalEntityName, _configuration.ManageApprenticeshipSiteUrl);
            return View(model);
        }
    }
}
