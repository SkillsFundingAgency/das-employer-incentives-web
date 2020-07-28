namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : ViewModel
    {
        public CannotApplyViewModel(string accountId, string commitmentsUrl, string title = "You cannot apply for this grant yet") : base(title)
        {
            AccountId = accountId;
            CommitmentsUrl = commitmentsUrl;            
        }

        public string AccountId { get; }
        public string CommitmentsUrl { get; }
    }
}
