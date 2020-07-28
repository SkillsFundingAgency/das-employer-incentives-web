using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class CreateApplicationRequest
    {
        public Guid ApplicationId { get; }
        public long AccountId { get; }
        public long AccountLegalEntityId { get; }
        public long[] ApprenticeshipIds { get; }
        
        public CreateApplicationRequest(Guid applicationId, long accountId, long accountLegalEntityId, IEnumerable<long> apprenticeshipIds)
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            ApprenticeshipIds = apprenticeshipIds.ToArray();
        }
    }
}
