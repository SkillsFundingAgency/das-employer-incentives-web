using SFA.DAS.EmployerIncentives.Web.Services.Email.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IApplicationService
    {
        Task<Guid> Create(string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds);
        Task<ApplicationConfirmationViewModel> Get(string accountId, Guid applicationId);
        Task Update(Guid applicationId, string accountId, IEnumerable<string> apprenticeshipIds);
        Task Confirm(string accountId, Guid applicationId, string userId);
        Task<long> GetApplicationLegalEntity(string accountId, Guid applicationId);
    }
}