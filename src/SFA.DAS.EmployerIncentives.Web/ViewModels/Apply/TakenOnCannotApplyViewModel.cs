namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class TakenOnCannotApplyViewModel : CannotApplyViewModel
    {
        public TakenOnCannotApplyViewModel(string accountId, string commitmentsBaseUrl, string accountsBaseUrl, string organisationName) : base(accountId, accountsBaseUrl, organisationName)
        {
            if (!commitmentsBaseUrl.EndsWith("/"))
            {
                commitmentsBaseUrl += "/";
            }
            AddApprenticesUrl = $"{commitmentsBaseUrl}{accountId}/unapproved/inform";
        }

        public new string Title => $"{OrganisationName} cannot apply for this payment";
    }
}
