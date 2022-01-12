using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class NewAgreementRequiredViewModel : IViewModel
    {
        public string Title => $"{OrganisationName} needs to accept a new employer agreement";

        public string OrganisationName { get; set; }

        private readonly string _accountsBaseUrl;
        public string AccountId { get; }
        public Guid ApplicationId { get; }

        public string AccountsAgreementsUrl => $"{_accountsBaseUrl}/accounts/{AccountId}/agreements";

        public NewAgreementRequiredViewModel(string organisationName, string accountId, Guid applicationId, string accountsBaseUrl)
        {
            _accountsBaseUrl = accountsBaseUrl;
            OrganisationName = organisationName;
            AccountId = accountId;
            ApplicationId = applicationId;
        }
    }
}