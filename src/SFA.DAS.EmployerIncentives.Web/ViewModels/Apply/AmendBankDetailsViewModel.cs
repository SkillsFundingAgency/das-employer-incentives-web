using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class AmendBankDetailsViewModel : ViewModel
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public Guid ApplicationId { get; set; }
        public string OrganisationName { get; set; }

        public AmendBankDetailsViewModel(string title, string accountId, string accountLegalEntityId, Guid applicationId, string organisationName) : base(title)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            ApplicationId = applicationId;
            OrganisationName = organisationName;
        }
    }
}
