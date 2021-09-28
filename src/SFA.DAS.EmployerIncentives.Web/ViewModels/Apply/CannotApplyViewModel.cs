namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : IViewModel
    {
        public CannotApplyViewModel(
            string accountId,
            string accountsBaseUrl,
            string organisationName)
        {
            AccountId = accountId;
            OrganisationName = organisationName;
            if (!accountsBaseUrl.EndsWith("/"))
            {
                accountsBaseUrl += "/";
            }
            AccountHomeUrl = $"{accountsBaseUrl}accounts/{AccountId}/teams";
        }

        public string AccountId { get; }
        public string AccountHomeUrl { get; }
        public string Title => $"{OrganisationName} does not have any eligible apprentices";
        public string OrganisationName { get; set; }
    }
}
