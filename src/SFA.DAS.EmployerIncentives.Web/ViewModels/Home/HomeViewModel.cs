namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel : IViewModel
    {
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }

        public string Title => "Apply for the hire a new apprentice payment";

        public string OrganisationName { get; set; }

        public HomeViewModel(string accountId, string accountLegalEntityId, string organisationName)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
        }

    }
}
