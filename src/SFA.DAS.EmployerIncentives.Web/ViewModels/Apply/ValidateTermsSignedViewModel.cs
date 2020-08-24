namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class ValidateTermsSignedViewModel : ViewModel
    {
        private readonly string _accountsBaseUrl;

        public ValidateTermsSignedViewModel(string accountId, string accountsBaseUrl, string title = "You need to accept the employer agreement") : base(title)
        {
            AccountId = accountId;
            _accountsBaseUrl = accountsBaseUrl;
        }

        public string AccountId { get; }
        public string AccountsHomeUrl => $"{_accountsBaseUrl}/accounts/{AccountId}/teams";
        public string AccountsAgreementsUrl => $"{_accountsBaseUrl}/accounts/{AccountId}/agreements";
    }
}
