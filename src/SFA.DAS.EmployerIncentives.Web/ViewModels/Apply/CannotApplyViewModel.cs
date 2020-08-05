namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : ViewModel
    {
        public CannotApplyViewModel(string accountId, string commitmentsBaseUrl, string title = "You cannot apply for this grant yet") : base(title)
        {
            AccountId = accountId;

            if (!commitmentsBaseUrl.EndsWith("/"))
            {
                commitmentsBaseUrl += "/";
            }
            CommitmentsUrl = $"{commitmentsBaseUrl}commitments/accounts/{accountId}/apprentices/home";
        }

        public string AccountId { get; }
        public string CommitmentsUrl { get; }
    }
}
