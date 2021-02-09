
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete
{
    public class ConfirmationViewModel : IViewModel
    {
        public ConfirmationViewModel(string accountsUrl)
        {
            AccountsUrl = accountsUrl;
        }

        public string AccountsUrl { get; }

        public string Title => "Application complete";

        public string OrganisationName { get; set; }
    }
}
