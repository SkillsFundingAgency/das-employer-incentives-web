using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IApprenticeshipIncentiveService
    {
        Task<IEnumerable<ApprenticeshipIncentiveModel>> GetList(string accountId, string accountLegalEntityId);
        Task Cancel(string accountLegalEntityId, IEnumerable<ApprenticeshipIncentiveModel> apprenticeshipIncentives);
    }
}
