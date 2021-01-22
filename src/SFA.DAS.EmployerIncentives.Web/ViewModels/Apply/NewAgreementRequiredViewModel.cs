using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class NewAgreementRequiredViewModel : ViewModel
    {
        private readonly string _accountsBaseUrl;

        public string OrganisationName { get; }
        public string AccountId { get; }
        public Guid ApplicationId { get; }

        public string AccountsAgreementsUrl => $"{_accountsBaseUrl}/accounts/{AccountId}/agreements";

        public NewAgreementRequiredViewModel(string organisationName, string accountId, Guid applicationId, string accountsBaseUrl) : base($"{organisationName} needs to accept a new employer agreement")
        {
            _accountsBaseUrl = accountsBaseUrl;
            OrganisationName = organisationName;
            AccountId = accountId;
            ApplicationId = applicationId;
        }
    }
}
