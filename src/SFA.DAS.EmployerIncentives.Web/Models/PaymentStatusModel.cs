using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class PaymentStatusModel
    {
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool LearnerMatchFound { get; set; }
        public bool HasDataLock { get; set; }
        public bool InLearning { get; set; }
        public bool PausePayments { get; set; }
        
        public bool ShowPaymentStatus
        {
            get
            {
                return !LearnerMatchFound || HasDataLock || !InLearning || PausePayments;
            }
        }
    }
}
