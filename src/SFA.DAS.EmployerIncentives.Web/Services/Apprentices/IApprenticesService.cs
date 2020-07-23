using System.Threading.Tasks;
using System.Collections.Generic;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities
{
    public interface IApprenticesService
    {
        Task<IEnumerable<ApprenticeshipModel>> Get(ApprenticesQuery query);
    }    
}
