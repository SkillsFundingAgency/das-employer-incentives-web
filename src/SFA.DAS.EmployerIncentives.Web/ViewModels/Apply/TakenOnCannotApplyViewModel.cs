namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class TakenOnCannotApplyViewModel : CannotApplyViewModel
    {
        public TakenOnCannotApplyViewModel(string accountId, string accountsBaseUrl, string commitmentsBaseUrl, string title = "You cannot apply for this payment") : base(accountId, accountsBaseUrl, commitmentsBaseUrl, title)
        {
            if (!commitmentsBaseUrl.EndsWith("/"))
            {
                commitmentsBaseUrl += "/";
            }
            AddApprenticesUrl = $"{commitmentsBaseUrl}commitments/accounts/{accountId}/apprentices/inform";
        }
    }
}
