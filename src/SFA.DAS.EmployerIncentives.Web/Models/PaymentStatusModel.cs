using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class PaymentStatusModel
    {
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool LearnerMatchNotFound { get; set; }
        public bool HasDataLock { get; set; }
        public bool ApprenticeNotInLearning { get; set; }

        public bool HasPaymentErrorStatus
        {
            get
            {
                return LearnerMatchNotFound || HasDataLock || ApprenticeNotInLearning;
            }
        }
    }
}
