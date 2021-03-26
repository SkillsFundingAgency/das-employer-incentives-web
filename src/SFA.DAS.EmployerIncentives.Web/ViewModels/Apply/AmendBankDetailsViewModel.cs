using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class AmendBankDetailsViewModel : IViewModel
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public Guid ApplicationId { get; set; }
        public string OrganisationName { get; set; }

        public string Title => $"Change {OrganisationName}'s organisation and finance details";

        public AmendBankDetailsViewModel()
        {
        }

        public AmendBankDetailsViewModel(string accountId, string accountLegalEntityId, Guid applicationId, string organisationName) 
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            ApplicationId = applicationId;
            OrganisationName = organisationName;
        }
    }
}
