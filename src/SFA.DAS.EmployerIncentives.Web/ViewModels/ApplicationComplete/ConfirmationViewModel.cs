
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete
{
    public class ConfirmationViewModel : ViewModel
    {
        public ConfirmationViewModel(string accountsUrl, string title = "Application complete") : base(title)
        {
            AccountsUrl = accountsUrl;
        }

        public string AccountsUrl { get; }
    }
}
