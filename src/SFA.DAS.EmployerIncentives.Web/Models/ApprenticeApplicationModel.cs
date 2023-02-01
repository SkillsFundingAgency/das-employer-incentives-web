using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class ApprenticeApplicationModel
    {
        public long AccountId { get; set; }
        public string LegalEntityName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ApprenticeName { get { return $"{FirstName} {LastName}"; } }
        public long ULN { get; set; }
        public DateTime ApplicationDate { get; set; }
        public decimal TotalIncentiveAmount { get; set; }
        public string Status { get; set; }
        public bool IsWithdrawn { 
            get
            {
                if ((FirstPaymentStatus != null) && (FirstPaymentStatus.WithdrawnByCompliance || FirstPaymentStatus.WithdrawnByEmployer))
                {
                    return true;
                }
                if (SecondPaymentStatus != null && (SecondPaymentStatus.WithdrawnByCompliance || SecondPaymentStatus.WithdrawnByEmployer))
                {
                    return true;
                }
                return false;
            }
        }
        public PaymentStatusModel FirstPaymentStatus { get; set; }
        public PaymentStatusModel SecondPaymentStatus { get; set; }
        public ClawbackStatusModel FirstClawbackStatus { get; set; }
        public ClawbackStatusModel SecondClawbackStatus { get; set; }
        public string CourseName { get; set; }
        public bool ShowSecondPaymentStatus => FirstPaymentStatus != null && !FirstPaymentStatus.ShowPaymentStatus;
        public bool IncentiveCompleted { get; set; }
    }
}
