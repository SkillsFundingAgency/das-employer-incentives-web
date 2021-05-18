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

        public Decimal TotalPaymentAmount { get; }

        public List<ApplicationApprenticeship> Apprentices { get; } 

        public bool BankDetailsRequired { get; }
        public bool NewAgreementRequired { get; }

        public string Title => "Confirm apprentices";

        public string OrganisationName { get; set; }
        public ApplicationConfirmationViewModel(Guid applicationId, string accountId, string accountLegalEntityId, 
            IEnumerable<ApplicationApprenticeship> apprentices, bool bankDetailsRequired, bool newAgreementRequired, string organisationName) 
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            Apprentices = apprentices.ToList();
            TotalPaymentAmount = Apprentices.Sum(x => x.ExpectedAmount);
            BankDetailsRequired = bankDetailsRequired;
            NewAgreementRequired = newAgreementRequired;
            OrganisationName = organisationName;
        }
    }
}