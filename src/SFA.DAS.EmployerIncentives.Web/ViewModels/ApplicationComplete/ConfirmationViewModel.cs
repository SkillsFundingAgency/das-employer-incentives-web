
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete
{
    public class ConfirmationViewModel : IViewModel
    {
        public ConfirmationViewModel(string accountId, string accountLegalEntityId, string organisationName, bool showBankDetailsInReview)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
            ShowBankDetailsInReview = showBankDetailsInReview;
        }

        public string Title => "Application complete";

        public string OrganisationName { get; set; }

        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public bool ShowBankDetailsInReview { get; }
    }
}
