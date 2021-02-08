namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel
    {
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public bool HasMultipleLegalEntities { get; }
        public string ManageApprenticeshipSiteUrl { get; }

        public string AccountHomeUrl { get; }
        public string OrganisationName { get; }

        public HomeViewModel(string accountId, string accountLegalEntityId, string organisationName, bool hasMultipleLegalEntities, string manageApprenticeshipSiteUrl)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
            HasMultipleLegalEntities = hasMultipleLegalEntities;
            ManageApprenticeshipSiteUrl = manageApprenticeshipSiteUrl;
            if (!manageApprenticeshipSiteUrl.EndsWith("/"))
            {
                manageApprenticeshipSiteUrl += "/";
            }
            AccountHomeUrl = $"{manageApprenticeshipSiteUrl}accounts/{AccountId}/teams";
        }
    }
}
