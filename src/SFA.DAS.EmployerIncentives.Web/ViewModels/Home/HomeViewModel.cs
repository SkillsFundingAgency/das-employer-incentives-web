namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel
    {
        public string AccountId { get; }

        public HomeViewModel(string accountId)
        {
            AccountId = accountId;
        }
    }
}
