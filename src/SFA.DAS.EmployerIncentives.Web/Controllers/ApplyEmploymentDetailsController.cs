using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
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
        private readonly ExternalLinksConfiguration _configuration;

        public ApplyEmploymentDetailsController(
            IApplicationService applicationService,
            ILegalEntitiesService legalEntityService,
            IHashingService hashingService,
            IEmploymentStartDateValidator employmentStartDateValidator,
            IOptions<ExternalLinksConfiguration> configuration) : base(legalEntityService)
        {
            _applicationService = applicationService;
            _hashingService = hashingService;
            _employmentStartDateValidator = employmentStartDateValidator;
            _configuration = configuration.Value;
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
        public async Task<IActionResult> SubmitEmploymentStartDates(EmploymentStartDatesRequest request)
        {
            var application = await _applicationService.Get(request.AccountId, request.ApplicationId, includeApprenticeships: true);
            var validationResults = _employmentStartDateValidator.Validate(request).ToList();
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

            await _applicationService.SaveApprenticeshipDetails(confirmEmploymentDetailsRequest);

            application = await _applicationService.Get(request.AccountId, request.ApplicationId, includeApprenticeships: false);

            if (application.NewAgreementRequired)
            {
                request.AccountLegalEntityId = application.AccountLegalEntityId;
                return await DisplayNewAgreementRequiredShutterPage(request);
            }

            return RedirectToAction("ConfirmApprenticeships", "ApplyApprenticeships", new { request.AccountId, request.ApplicationId });
        }

        private async Task<IActionResult> DisplayNewAgreementRequiredShutterPage(EmploymentStartDatesRequest request)
        {
            var legalEntityName = await GetLegalEntityName(request.AccountId, request.AccountLegalEntityId);
            var viewModel = new NewAgreementRequiredViewModel(legalEntityName, request.AccountId, request.ApplicationId,
                _configuration.ManageApprenticeshipSiteUrl);
            return View("NewAgreementRequired", viewModel);
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
                var apprentice = apprentices.Single(x => x.ApprenticeshipId == request.ApprenticeshipIds[index]);
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

        private ApprenticeshipDetailsRequest CreateEmploymentDetailsRequest(ApplicationModel application, EmploymentStartDatesRequest request)
        {
            var confirmRequest = new ApprenticeshipDetailsRequest
            {
                AccountId = _hashingService.DecodeValue(application.AccountId),
                ApplicationId = application.ApplicationId,
                ApprenticeshipDetails = new List<ApprenticeshipDetailsDto>()
            };

            for(var index = 0; index < request.EmploymentStartDateDays.Count; index ++)
            {
                var employmentStartDate = new DateTime(request.EmploymentStartDateYears[index].Value, request.EmploymentStartDateMonths[index].Value, request.EmploymentStartDateDays[index].Value);
                var employmentDetails = new ApprenticeshipDetailsDto
                {
                    ApprenticeId = _hashingService.DecodeValue(request.ApprenticeshipIds[index]),
                    EmploymentStartDate = employmentStartDate
                };
                confirmRequest.ApprenticeshipDetails.Add(employmentDetails);
            }

            return confirmRequest;
        }
    }
}
