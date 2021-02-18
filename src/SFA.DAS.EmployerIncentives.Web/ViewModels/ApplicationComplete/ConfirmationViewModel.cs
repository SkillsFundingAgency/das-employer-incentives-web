
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete
{
    public class ConfirmationViewModel : IViewModel
    {
        public ConfirmationViewModel(string accountId, string accountLegalEntityId, string organisationName, string accountsUrl)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
            AccountsUrl = accountsUrl;
        }

        public string Title => "Application saved";

        public string OrganisationName { get; set; }

        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public string AccountsUrl { get; }
    }
}
