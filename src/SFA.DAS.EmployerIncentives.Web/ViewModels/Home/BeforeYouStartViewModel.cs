namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{    public class BeforeYouStartViewModel : HomeViewModel
    {
        public BeforeYouStartViewModel(string accountId,
            string accountLegalEntityId,
            string organisationName,
            bool newAgreementRequired,
            string manageApprenticeshipSiteUrl,
            bool bankDetailsRequired) : base(accountId, accountLegalEntityId, organisationName, newAgreementRequired, manageApprenticeshipSiteUrl, bankDetailsRequired)
        {
            Title = "Before you start"; 
        }
    }
}
