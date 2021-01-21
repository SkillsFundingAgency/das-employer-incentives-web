namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : ViewModel
    {
        public CannotApplyViewModel(
            string accountId,
            string accountsBaseUrl,
            string title = "You do not have any eligible apprentices"
        ) : base(title)
        {
            AccountId = accountId;

            if (!accountsBaseUrl.EndsWith("/"))
            {
                accountsBaseUrl += "/";
            }
            AccountHomeUrl = $"{accountsBaseUrl}accounts/{AccountId}/teams";
        }

        public string AccountId { get; }
        public string AccountHomeUrl { get; }
        public string AddApprenticesUrl { get; set; }
    }
}
