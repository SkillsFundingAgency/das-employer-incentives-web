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
        public bool HasIneligibleApprentices { get; }

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
            TotalPaymentAmount = Apprentices.Where(x => x.HasEligibleEmploymentStartDate).Sum(x => x.ExpectedAmount);
            BankDetailsRequired = bankDetailsRequired;
            OrganisationName = organisationName;
            HasIneligibleApprentices = Apprentices.Any(a => !a.HasEligibleEmploymentStartDate);
        }
    }
}