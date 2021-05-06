using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class ApplyEmploymentDetailsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplyEmploymentDetailsController(
            IApplicationService applicationService,
            ILegalEntitiesService legalEntityService) : base(legalEntityService)
        {
            _applicationService = applicationService;
        }

        public async Task<IActionResult> EmploymentStartDates(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: true);
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            var model = new EmploymentStartDatesViewModel();
            return View(model);
        }
    }
}
