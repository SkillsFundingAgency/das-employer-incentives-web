using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class ApplicationConfirmationViewModel : ViewModel
    {
        public Guid ApplicationId { get; }
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }

        public Decimal TotalPaymentAmount { get; }

        public List<ApplicationApprenticeship> Apprentices { get; } 

        public ApplicationConfirmationViewModel(Guid applicationId, string accountId, string accountLegalEntityId, IEnumerable<ApplicationApprenticeship> apprentices) : base("Confirm your apprentices")
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            Apprentices = apprentices.ToList();
            TotalPaymentAmount = Apprentices.Sum(x => x.ExpectedAmount);
        }

        public class ApplicationApprenticeship
        {
            public long ApprenticeshipId { get; set; }
            public string DisplayName => $"{FirstName} {LastName}";
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CourseName { get; set; }
            public decimal ExpectedAmount { get; set; }
        }
    }
}