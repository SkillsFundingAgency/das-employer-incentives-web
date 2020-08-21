using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class ConfirmApplicationRequest
    {
        public Guid ApplicationId { get; }
        public long AccountId { get; }
        public DateTime DateSubmitted { get; set; }
        public string SubmittedByEmail { get; set; }
        public string SubmittedByName { get; set; }

        public ConfirmApplicationRequest(Guid applicationId, long accountId, string submittedByEmail, string submittedByName)
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            SubmittedByEmail = submittedByEmail;
            SubmittedByName = submittedByName;
            DateSubmitted = DateTime.UtcNow;
        }
    }
}
