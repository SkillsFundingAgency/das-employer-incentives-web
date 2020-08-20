namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : ViewModel
    {
        public CannotApplyViewModel(
            string accountId,
            string accountsBaseUrl,
            string title = "You can only apply for apprentices who started their contract of employment between 1 August 2020 and 31 January 2021"
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
