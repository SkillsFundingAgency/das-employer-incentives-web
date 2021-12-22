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
        public bool PaymentSent { get; set; }
        public bool PaymentSentIsEstimated { get; set; }
        public bool RequiresNewEmployerAgreement { get; set; }
        public string ViewAgreementLink { get; set; }
        public bool PaymentIsStopped { get; set; }
        public bool WithdrawnByEmployer { get; set; }
        public bool WithdrawnByCompliance { get; set; }
        public bool IsClawedBack { get; set; }
        public bool EmploymentCheckPassed { get; set; }

        public bool DisplayEmploymentCheckResult { get; set; }

        public bool ShowPaymentStatus
        {
            get
            {
                return PaymentIsStopped || !LearnerMatchFound || HasDataLock || !InLearning || PausePayments || RequiresNewEmployerAgreement || WithdrawnByCompliance || WithdrawnByEmployer || (DisplayEmploymentCheckResult && !EmploymentCheckPassed);
            }
        }
    }
}
