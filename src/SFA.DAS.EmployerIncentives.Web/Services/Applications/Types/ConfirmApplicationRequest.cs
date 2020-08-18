using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class ConfirmApplicationRequest
    {
        public Guid ApplicationId { get; }
        public long AccountId { get; }
        public DateTime DateSubmitted { get; set; }
        public string SubmittedBy { get; set; }

        public ConfirmApplicationRequest(Guid applicationId, long accountId, string submittedBy)
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            SubmittedBy = submittedBy;
            DateSubmitted = DateTime.UtcNow;
        }
    }
}
