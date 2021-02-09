namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel : IViewModel
    {
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public bool HasMultipleLegalEntities { get; }
        public string ManageApprenticeshipSiteUrl { get; }

        public string AccountHomeUrl { get; }

        public string Title => "Apply for the hire a new apprentice payment";

        public string OrganisationName { get; set; }

        public HomeViewModel(string accountId, string accountLegalEntityId, string organisationName, bool hasMultipleLegalEntities, string manageApprenticeshipSiteUrl) 
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            HasMultipleLegalEntities = hasMultipleLegalEntities;
            ManageApprenticeshipSiteUrl = manageApprenticeshipSiteUrl;
            OrganisationName = organisationName;
            if (!manageApprenticeshipSiteUrl.EndsWith("/"))
            {
                manageApprenticeshipSiteUrl += "/";
            }
            AccountHomeUrl = $"{manageApprenticeshipSiteUrl}accounts/{AccountId}/teams";
        }
    }
}
