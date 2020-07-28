using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IApplicationService
    {
        Task<Guid> Post(string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds);
        Task<ApplicationConfirmationViewModel> Get(string accountId, Guid applicationId);

    }    
}
