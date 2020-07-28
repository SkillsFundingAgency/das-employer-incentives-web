using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public class CreateDraftSubmission
    {
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public List<string> ApprenticeshipIds { get; }
        
        public CreateDraftSubmission(string accountId, string accountLegalEntityId, List<string> apprenticeshipIds)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            ApprenticeshipIds = apprenticeshipIds;
        }
    }
}
