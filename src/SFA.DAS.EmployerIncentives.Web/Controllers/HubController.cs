﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Hub;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class HubController : Controller
    {
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly IApplicationService _applicationService;
        private readonly ExternalLinksConfiguration _externalLinksConfiguration;
        private readonly WebConfigurationOptions _webConfiguration;

        public HubController(ILegalEntitiesService legalEntitiesService, IApplicationService applicationService, 
                             IOptions<ExternalLinksConfiguration> externalLinksConfiguration, IOptions<WebConfigurationOptions> webConfiguration)
        {
            _legalEntitiesService = legalEntitiesService;
            _applicationService = applicationService;
            _externalLinksConfiguration = externalLinksConfiguration.Value;
            _webConfiguration = webConfiguration.Value;
        }

        [Route("{accountId}/{accountLegalEntityId}/hire-new-apprentice-payment")]
        [HttpGet]
        public async Task<IActionResult> Index(string accountId, string accountLegalEntityId)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);
            var selectedLegalEntity = legalEntities.FirstOrDefault(x => x.AccountLegalEntityId == accountLegalEntityId);

            var model = new HubPageViewModel(_externalLinksConfiguration.ManageApprenticeshipSiteUrl, accountId)
            {
                AccountLegalEntityId = accountLegalEntityId,
                OrganisationName = selectedLegalEntity?.Name,
                HasMultipleLegalEntities = legalEntities.Count() > 1
            };

            var applicationsResponse = await _applicationService.GetList(accountId, accountLegalEntityId);
            if (applicationsResponse.ApprenticeApplications.Any())
            {
                model.ShowBankDetailsRequired = BankDetailsRequired(applicationsResponse);
                model.ShowAmendBankDetails = CanAmendBankDetails(applicationsResponse);
                model.BankDetailsApplicationId = applicationsResponse.FirstSubmittedApplicationId.Value;
                model.ShowAcceptNewEmployerAgreement = applicationsResponse.ApprenticeApplications.Any(a => (a.FirstPaymentStatus != null && a.FirstPaymentStatus.RequiresNewEmployerAgreement) || (a.SecondPaymentStatus != null && a.SecondPaymentStatus.RequiresNewEmployerAgreement));
                model.ShowPhaseTwoClosureContent = ShowPhaseTwoClosureContent(_webConfiguration.ApplicationShutterPageDate);
            }

            return View(model);
        }

        private static bool BankDetailsRequired(GetApplicationsModel applications)
        {
            return applications.BankDetailsStatus == BankDetailsStatus.NotSupplied || applications.BankDetailsStatus == BankDetailsStatus.Rejected;
        }
        private static bool CanAmendBankDetails(GetApplicationsModel applications)
        {
            return applications.BankDetailsStatus == BankDetailsStatus.Completed;
        }
        private bool ShowPhaseTwoClosureContent(string applicationShutterPageDate)
        {
            DateTime applyFromDate;
            DateTime.TryParse(applicationShutterPageDate, out applyFromDate);
            return applyFromDate != DateTime.MinValue && DateTime.Today >= applyFromDate.Date;
        }

    }
}
