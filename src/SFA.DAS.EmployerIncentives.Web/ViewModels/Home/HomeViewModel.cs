namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel
    {
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }

        public HomeViewModel(string accountId, string accountLegalEntityId)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
        }
    }
}
