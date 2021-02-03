using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Hub;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class HubController : Controller
    {
        private ILegalEntitiesService _legalEntitiesService;
        private IApprenticeshipIncentiveService _applicationService;
        private ExternalLinksConfiguration _configuration;

        public HubController(ILegalEntitiesService legalEntitiesService, IApprenticeshipIncentiveService applicationService, IOptions<ExternalLinksConfiguration> configuration)
        {
            _legalEntitiesService = legalEntitiesService;
            _applicationService = applicationService;
            _configuration = configuration.Value;
        }

        [Route("{accountId}/{accountLegalEntityId}/hire-new-apprentice-payment")]
        public async Task<IActionResult> Index(string accountId, string accountLegalEntityId)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);
            var selectedLegalEntity = legalEntities.FirstOrDefault(x => x.AccountLegalEntityId == accountLegalEntityId);

            var model = new HubPageViewModel(_configuration.ManageApprenticeshipSiteUrl, accountId)
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
    }
}
