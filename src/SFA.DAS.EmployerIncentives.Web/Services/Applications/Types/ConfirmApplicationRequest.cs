using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class ConfirmApplicationRequest
    {
        public Guid ApplicationId { get; }
        public long AccountId { get; }
        public DateTime DateSubmitted { get; set; }

        public ConfirmApplicationRequest(Guid applicationId, long accountId)
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            DateSubmitted = DateTime.UtcNow;
        }
    }
}
