namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel : IViewModel
    {
        public string Title { get; protected set; } 

        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public bool NewAgreementRequired { get; }
        public bool BankDetailsRequired { get; }        
        public string AccountsAgreementsUrl { get; }
        public string OrganisationName { get; set; }

        private string ManageApprenticeshipSiteUrl { get; }        

        public HomeViewModel(
            string accountId, 
            string accountLegalEntityId, 
            string organisationName,             
            bool newAgreementRequired, 
            string manageApprenticeshipSiteUrl,
            bool bankDetailsRequired)
        {
            Title = "Apply for the hire a new apprentice payment";
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
            NewAgreementRequired = newAgreementRequired;
            ManageApprenticeshipSiteUrl = manageApprenticeshipSiteUrl;
            if (!manageApprenticeshipSiteUrl.EndsWith("/"))
            {
                ManageApprenticeshipSiteUrl += "/";
            }
            AccountsAgreementsUrl = $"{ManageApprenticeshipSiteUrl}accounts/{AccountId}/teams";
            BankDetailsRequired = bankDetailsRequired;
        }
    }
}
