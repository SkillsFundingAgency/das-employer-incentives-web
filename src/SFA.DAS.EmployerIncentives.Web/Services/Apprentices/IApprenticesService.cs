using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public interface IApprenticesService
    {
        Task<EligibleApprenticeshipsModel> Get(ApprenticesQuery query);
    }    
}
