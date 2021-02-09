namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class ValidateTermsSignedViewModel : IViewModel
    {
        private readonly string _accountsBaseUrl;

        public ValidateTermsSignedViewModel(string accountId, string accountsBaseUrl)
        {
            AccountId = accountId;
            _accountsBaseUrl = accountsBaseUrl;
        }

        public string AccountId { get; }
        public string AccountsHomeUrl => $"{_accountsBaseUrl}/accounts/{AccountId}/teams";
        public string AccountsAgreementsUrl => $"{_accountsBaseUrl}/accounts/{AccountId}/agreements";

        public string Title => $"{OrganisationName} needs to accept the employer agreement";

        public string OrganisationName { get; set; }
    }
}
