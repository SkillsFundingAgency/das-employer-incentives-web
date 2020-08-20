namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Error
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string accountsBaseUrl)
        {
            if (!accountsBaseUrl.EndsWith("/"))
            {
                accountsBaseUrl += "/";
            }
            AccountHomeUrl = $"{accountsBaseUrl}";
        }

        public string AccountHomeUrl { get; }
    }
}
