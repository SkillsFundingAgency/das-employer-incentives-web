using SFA.DAS.EmployerIncentives.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IApplicationService
    {
        Task<Guid> Create(string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds);
        Task<ApplicationModel> Get(string accountId, Guid applicationId, bool includeApprenticeships = true);
        Task Update(Guid applicationId, string accountId, IEnumerable<string> apprenticeshipIds);
        Task Confirm(string accountId, Guid applicationId, string userEmail, string userName);
        Task<long> GetApplicationLegalEntity(string accountId, Guid applicationId);
        Task SaveApprenticeshipDetails(ApprenticeshipDetailsRequest request);
        Task<GetApplicationsModel> GetList(string accountId, string accountLegalEntityId);
    }
}