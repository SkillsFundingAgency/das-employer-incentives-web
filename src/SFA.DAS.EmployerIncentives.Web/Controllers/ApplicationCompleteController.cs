using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;
using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.BankDetails;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/application-complete/{applicationId}")]
    public class ApplicationCompleteController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IBankDetailsStatusService _bankDetailsStatusService;
        private const string VrfStatusCompleted = "Case Request completed";

        public ApplicationCompleteController(ILegalEntitiesService legalEntitiesService,
                                             IApplicationService applicationService,
                                             IBankDetailsStatusService bankDetailsStatusService)
            : base(legalEntitiesService)
        {
            _bankDetailsStatusService = bankDetailsStatusService;
            _applicationService = applicationService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Confirmation(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);
            var legalEntity = await _bankDetailsStatusService.RecordBankDetailsComplete(accountId, application.AccountLegalEntityId);
            var showBankDetailsInReview = (!String.IsNullOrWhiteSpace(legalEntity.VrfCaseStatus) && legalEntity.VrfCaseStatus != VrfStatusCompleted);
            var model = new ConfirmationViewModel(accountId, application.AccountLegalEntityId, legalEntity.Name, showBankDetailsInReview);
            return View(model);
        }
    }
}
