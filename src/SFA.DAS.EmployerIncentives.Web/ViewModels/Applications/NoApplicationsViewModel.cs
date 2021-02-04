
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Applications
{
    public class NoApplicationsViewModel : ViewModel
    {
        public NoApplicationsViewModel(string accountId, string accountLegalEntityId) : base("Your hire a new apprentice payment applications")
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
        }

        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
    }
}
