using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class ConfirmApplicationRequest
    {
        public Guid IncentiveApplicationId { get; }
        public long AccountId { get; }
        public DateTime DateSubmitted { get; set; }
        public string SubmittedBy { get; set; }

        public ConfirmApplicationRequest(Guid incentiveApplicationId, long accountId, string submittedBy)
        {
            IncentiveApplicationId = incentiveApplicationId;
            AccountId = accountId;
            SubmittedBy = submittedBy;
            DateSubmitted = DateTime.UtcNow;
        }
    }
}
