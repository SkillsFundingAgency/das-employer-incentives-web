using Microsoft.AspNetCore.Mvc;
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
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly IApplicationService _applicationService;
        private const string VrfStatusRequested = "Requested";
        private const string VrfStatusCompleted = "Case Request completed";
        public ApplicationCompleteController(ILegalEntitiesService legalEntitiesService,
                                             IApplicationService applicationService)
            : base(legalEntitiesService)
        {
            _legalEntitiesService = legalEntitiesService;
            _applicationService = applicationService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Confirmation(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);
            var legalEntity = await _legalEntitiesService.Get(accountId, application.AccountLegalEntityId);
            if (String.IsNullOrWhiteSpace(legalEntity.VrfCaseStatus))
            {
                legalEntity.VrfCaseStatus = VrfStatusRequested;
                await _legalEntitiesService.UpdateVrfCaseStatus(legalEntity);
            }
            var showBankDetailsInReview = (!String.IsNullOrWhiteSpace(legalEntity.VrfCaseStatus) && legalEntity.VrfCaseStatus != VrfStatusCompleted);
            var model = new ConfirmationViewModel(accountId, application.AccountLegalEntityId, legalEntity.Name, showBankDetailsInReview);
            return View(model);
        }
    }
}
