namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel
    {
        private readonly string _accountsBaseUrl;

        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public string OrganisationName { get; }
        public bool NewAgreementRequired { get; }
        
        public string AccountsAgreementsUrl => $"{_accountsBaseUrl}/accounts/{AccountId}/agreements";

        public HomeViewModel(string accountId, string accountLegalEntityId, string organisationName, bool newAgreementRequired, string accountsBaseUrl)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
            NewAgreementRequired = newAgreementRequired;
            _accountsBaseUrl = accountsBaseUrl;
        }
    }
}
