using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyEmploymentDetailsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IHashingService _hashingService;

        public ApplyEmploymentDetailsController(
            IApplicationService applicationService,
            ILegalEntitiesService legalEntityService,
            IHashingService hashingService) : base(legalEntityService)
        {
            _applicationService = applicationService;
            _hashingService = hashingService;
        }

        [Route("{applicationId}/join-organisation")]
        [HttpGet]
        public async Task<IActionResult> EmploymentStartDates(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: true);
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            var model = new EmploymentStartDatesViewModel
            {
                AccountId = accountId,
                ApplicationId = applicationId,
                OrganisationName = legalEntityName,
                Apprentices = application.Apprentices 
            };
            return View(model);
        }

        [Route("{applicationId}/join-organisation")]
        [HttpPost]
        public async Task<IActionResult> SubmitEmploymentStartDates(EmploymentStartDatesRequest request)
        {
            var application = await _applicationService.Get(request.AccountId, request.ApplicationId, includeApprenticeships: true);
            var confirmEmploymentDetailsRequest = CreateEmploymentDetailsRequest(application, request);
            await _applicationService.ConfirmEmploymentDetails(confirmEmploymentDetailsRequest);

            return RedirectToAction("ConfirmApprenticeships", "ApplyApprenticeships", new { request.AccountId, request.ApplicationId });
        }

        private ConfirmEmploymentDetailsRequest CreateEmploymentDetailsRequest(ApplicationModel application, EmploymentStartDatesRequest request)
        {
            var confirmRequest = new ConfirmEmploymentDetailsRequest
            {
                AccountId = _hashingService.DecodeValue(application.AccountId),
                ApplicationId = application.ApplicationId,
                EmploymentDetails = new List<ApprenticeEmploymentDetailsDto>()
            };

            for(var index = 0; index < request.EmploymentStartDateDays.Count; index ++)
            {
                var employmentStartDate = new DateTime(request.EmploymentStartDateYears[index], request.EmploymentStartDateMonths[index], request.EmploymentStartDateDays[index]);
                var employmentDetails = new ApprenticeEmploymentDetailsDto
                {
                    ApprenticeId = _hashingService.DecodeValue(application.Apprentices[index].ApprenticeshipId),
                    EmploymentStartDate = employmentStartDate
                };
                confirmRequest.EmploymentDetails.Add(employmentDetails);
            }

            return confirmRequest;
        }
    }
}
