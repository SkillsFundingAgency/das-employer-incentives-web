using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class ApplicationModel
    {
        public Guid ApplicationId { get; }
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }

        public Decimal TotalPaymentAmount { get; }

        public List<ApplicationApprenticeshipModel> Apprentices { get; }

        public bool BankDetailsRequired { get; }
        public bool NewAgreementRequired { get; }

        public string OrganisationName { get; set; }
        public ApplicationModel(Guid applicationId, string accountId, string accountLegalEntityId, IEnumerable<ApplicationApprenticeshipModel> apprentices, bool bankDetailsRequired, bool newAgreementRequired)
        {
            ApplicationId = applicationId;
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            Apprentices = apprentices.ToList();
            TotalPaymentAmount = Apprentices.Sum(x => x.ExpectedAmount);
            BankDetailsRequired = bankDetailsRequired;
            NewAgreementRequired = newAgreementRequired;
        }
    }
}
