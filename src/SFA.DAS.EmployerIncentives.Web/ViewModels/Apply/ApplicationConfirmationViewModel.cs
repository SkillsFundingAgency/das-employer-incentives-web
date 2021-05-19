using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class ApplicationConfirmationViewModel : IViewModel
    {
        public Guid ApplicationId { get; }
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }

        public decimal TotalPaymentAmount { get; }

        public List<ApplicationApprenticeship> Apprentices { get; } 

        public bool BankDetailsRequired { get; }

        public string Title => "Confirm apprentices";

        public string OrganisationName { get; set; }
        public ApplicationConfirmationViewModel(Guid applicationId, string accountId, string accountLegalEntityId, 
            IEnumerable<ApplicationApprenticeship> apprentices, bool bankDetailsRequired, string organisationName) 
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            Apprentices = apprentices.ToList();
            TotalPaymentAmount = Apprentices.Sum(x => x.ExpectedAmount);
            BankDetailsRequired = bankDetailsRequired;
            OrganisationName = organisationName;
        }

        public class ApplicationApprenticeship
        {
            public string ApprenticeshipId { get; set; }
            public string DisplayName => $"{FirstName} {LastName}";
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CourseName { get; set; }
            public DateTime StartDate { get; set; }
            public decimal ExpectedAmount { get; set; }
            public long Uln { get; set; }
        }
    }
}