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
    [Route("{accountId}/complete/{applicationId}")]
    public class ApplicationCompleteController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationCompleteController(ILegalEntitiesService legalEntitiesService,
                                             IApplicationService applicationService)
            : base(legalEntitiesService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        [Route("application-saved")]
        public async Task<IActionResult> Confirmation(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            var model = new ConfirmationViewModel(accountId, application.AccountLegalEntityId, legalEntityName);
            return View(model);
        }
    }
}
