
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class NeedBankDetailsViewModel : IViewModel
    {
        public string Title => "Application saved";

        public string OrganisationName { get; set; }
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }

        public NeedBankDetailsViewModel(string accountId, string accountLegalEntityId, string organisationName)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
        }
    }
}
