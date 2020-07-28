using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public interface IApprenticesService
    {
        Task<IEnumerable<ApprenticeshipModel>> Get(ApprenticesQuery query);
        Task<string> CreateDraftSubmission(CreateDraftSubmission draftSubmission);
    }    
}
