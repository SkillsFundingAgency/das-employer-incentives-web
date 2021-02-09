using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class ControllerBase : Controller
    {
        private readonly ILegalEntitiesService _legalEntitiesService;
        protected ControllerBase(ILegalEntitiesService legalEntitiesService)
        {
            _legalEntitiesService = legalEntitiesService;
        }

        protected async Task<string> GetLegalEntityName(string accountId, string accountLegalEntityId)
        {
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);
            return legalEntity != null ? legalEntity.Name : string.Empty;
        }

        protected async Task<bool> HasMultipleLegalEntities(string accountId)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);
            return legalEntities.Count() > 1;
        }
    }
}
