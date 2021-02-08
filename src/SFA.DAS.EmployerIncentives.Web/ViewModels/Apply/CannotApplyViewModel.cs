namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : ViewModel
    {
        public CannotApplyViewModel(
            string accountId,
            string accountsBaseUrl,
            string title,
            string organisationName
        ) : base(title)
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
        public string AddApprenticesUrl { get; set; }
        public string OrganisationName { get; set; }
    }
}
