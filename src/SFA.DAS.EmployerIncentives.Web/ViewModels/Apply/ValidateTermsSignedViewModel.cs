namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class ValidateTermsSignedViewModel : ViewModel
    {
        public ValidateTermsSignedViewModel(string accountId, string accountsBaseUrl, string title = "You need to accept the employer agreement") : base(title)
        {
            AccountId = accountId;
            _accountsBaseUrl = accountsBaseUrl;
        }

        private string _accountsBaseUrl;

        public string AccountId { get; }
        public string AccountsHomeUrl => $"{_accountsBaseUrl}{AccountId}/teams";
        public string AccountsAgreementsUrl => $"{_accountsBaseUrl}{AccountId}/agreements";
    }
}
