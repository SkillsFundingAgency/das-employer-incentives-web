using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;

namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities
{
    public interface IApprenticesService
    {
        Task<IEnumerable<ApprenticeDto>> Get(ApprenticesQuery query);
    }    
}
