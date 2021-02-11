namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel
    {
        private readonly string _accountsBaseUrl;

        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public string OrganisationName { get; }
        public bool NewAgreementRequired { get; }
        public bool HasMultipleLegalEntities { get; }
        public string ManageApprenticeshipSiteUrl { get; }

        public string AccountHomeUrl { get; }
        
        public string AccountsAgreementsUrl => $"{ManageApprenticeshipSiteUrl}accounts/{AccountId}/agreements";

        public HomeViewModel(string accountId, string accountLegalEntityId, string organisationName, bool hasMultipleLegalEntities, bool newAgreementRequired, string manageApprenticeshipSiteUrl)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
            NewAgreementRequired = newAgreementRequired;
            ManageApprenticeshipSiteUrl = manageApprenticeshipSiteUrl;
            HasMultipleLegalEntities = hasMultipleLegalEntities;
            if (!ManageApprenticeshipSiteUrl.EndsWith("/"))
            {
                ManageApprenticeshipSiteUrl += "/";
            }
            AccountHomeUrl = $"{ManageApprenticeshipSiteUrl}accounts/{AccountId}/teams";
        }

    }
}
