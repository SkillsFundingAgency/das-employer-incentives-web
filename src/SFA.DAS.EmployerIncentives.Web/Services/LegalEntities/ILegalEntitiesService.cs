using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities
{
    public interface ILegalEntitiesService
    {
        Task<IEnumerable<LegalEntityDto>> Get(long accountId);
    }
}
