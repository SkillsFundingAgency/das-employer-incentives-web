using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IApprenticeshipIncentiveService
    {
        Task<GetApplicationsModel> GetList(string accountId, string accountLegalEntityId);
    }
}
