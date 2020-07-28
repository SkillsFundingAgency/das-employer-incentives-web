using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IApplicationService
    {
        Task<Guid> Post(string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds);
    }    
}
