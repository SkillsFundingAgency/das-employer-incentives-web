using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Hub;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class HubController : Controller
    {
        private ILegalEntitiesService _legalEntitiesService;
        private ExternalLinksConfiguration _configuration;

        public HubController(ILegalEntitiesService legalEntitiesService, IOptions<ExternalLinksConfiguration> configuration)
        {
            _legalEntitiesService = legalEntitiesService;
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

            return View(model);
        }
    }
}
