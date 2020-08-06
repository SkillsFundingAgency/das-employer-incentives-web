namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class TakenOnCannotApplyViewModel : CannotApplyViewModel
    {
        public TakenOnCannotApplyViewModel(string accountId, string commitmentsBaseUrl, string title = "You cannot apply for this grant") : base(accountId, commitmentsBaseUrl, title)
        {
        }
    }
}
