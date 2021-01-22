using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities
{
    public interface ILegalEntitiesService
    {
        Task<IEnumerable<LegalEntityModel>> Get(string accountId);
        Task<LegalEntityModel> Get(string accountId, string accountLegalEntityId);
        Task<LegalEntityModel> Get(string hashedAccountId, long accountLegalEntityId);
    }
}
