using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class HubController : Controller
    {
        private ILegalEntitiesService _legalEntitiesService;

        public HubController(ILegalEntitiesService legalEntitiesService)
        {
            _legalEntitiesService = legalEntitiesService;
        }

        [Route("{accountId}/{accountLegalEntityId}/hire-new-apprentice-payment")]
        public async Task<IActionResult> Index(string accountId, string accountLegalEntityId)
        {
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);

            var model = new HubPageViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                OrganisationName = legalEntity?.Name
            };

            return View(model);
        }
    }
}
