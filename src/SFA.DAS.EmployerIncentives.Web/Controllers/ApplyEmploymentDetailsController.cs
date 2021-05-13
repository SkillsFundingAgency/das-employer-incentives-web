using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Validators;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyEmploymentDetailsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IHashingService _hashingService;
        private readonly IEmploymentStartDateValidator _employmentStartDateValidator;

        public ApplyEmploymentDetailsController(
            IApplicationService applicationService,
            ILegalEntitiesService legalEntityService,
            IHashingService hashingService,
            IEmploymentStartDateValidator employmentStartDateValidator) : base(legalEntityService)
        {
            _applicationService = applicationService;
            _hashingService = hashingService;
            _employmentStartDateValidator = employmentStartDateValidator;
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
                Apprentices = application.Apprentices.OrderBy(x => x.FullName).ToList(),
                DateValidationResults = new List<DateValidationResult>()
            };
            return View(model);
        }

        [Route("{applicationId}/join-organisation")]
        [HttpPost]
        public async Task<IActionResult> SubmitEmploymentStartDates([FromBody] EmploymentStartDatesRequest request)
        {
            var application = await _applicationService.Get(request.AccountId, request.ApplicationId, includeApprenticeships: true);
            var validationResults = _employmentStartDateValidator.Validate(request);
            if (validationResults.Any()) 
            {
                var legalEntityName = await GetLegalEntityName(request.AccountId, application.AccountLegalEntityId);
                var model = new EmploymentStartDatesViewModel
                {
                    AccountId = request.AccountId,
                    ApplicationId = request.ApplicationId,
                    OrganisationName = legalEntityName,
                    Apprentices = PopulateStartDates(application.Apprentices.OrderBy(x => x.FullName).ToList(), request),
                    DateValidationResults = validationResults.ToList()
                };
                return View("EmploymentStartDates", model);
            }

            var confirmEmploymentDetailsRequest = CreateEmploymentDetailsRequest(application, request);
            await _applicationService.ConfirmEmploymentDetails(confirmEmploymentDetailsRequest);

            return RedirectToAction("ConfirmApprenticeships", "ApplyApprenticeships", new { request.AccountId, request.ApplicationId });
        }

        private List<ApplicationApprenticeshipModel> PopulateStartDates(List<ApplicationApprenticeshipModel> apprentices, EmploymentStartDatesRequest request)
        {
            // dates may be partially filled out so find the column with the most values
            var startDateCount = new List<int> 
            {
                request.EmploymentStartDateDays.Count, 
                request.EmploymentStartDateMonths.Count, 
                request.EmploymentStartDateYears.Count
            }.Max();

            for (var index = 0; index < startDateCount; index++)
            {
                var apprentice = apprentices[index];
                if (request.EmploymentStartDateDays.Count > index)
                {
                    apprentice.EmploymentStartDateDay = request.EmploymentStartDateDays[index];
                }
                if (request.EmploymentStartDateMonths.Count > index)
                {
                    apprentice.EmploymentStartDateMonth = request.EmploymentStartDateMonths[index];
                }
                if (request.EmploymentStartDateYears.Count > index)
                {
                    apprentice.EmploymentStartDateYear = request.EmploymentStartDateYears[index];
                }
            }

            return apprentices;
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
                var employmentStartDate = new DateTime(request.EmploymentStartDateYears[index].Value, request.EmploymentStartDateMonths[index].Value, request.EmploymentStartDateDays[index].Value);
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
