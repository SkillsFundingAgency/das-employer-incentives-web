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
            AddApprenticesUrl = $"{commitmentsBaseUrl}commitments/accounts/{accountId}/apprentices/inform";
        }

        public new string Title => $"{OrganisationName} does not have any eligible apprentices";
    }
}
