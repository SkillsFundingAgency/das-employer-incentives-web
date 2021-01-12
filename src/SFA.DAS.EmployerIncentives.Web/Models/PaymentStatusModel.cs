using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class PaymentStatusModel
    {
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool LearnerMatchNotFound { get; set; }
    }
}
