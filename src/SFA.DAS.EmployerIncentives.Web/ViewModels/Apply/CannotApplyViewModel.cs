namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : ViewModel
    {
        public CannotApplyViewModel(string commitmentsUrl, string title = "You cannot apply for this grant yet") : base(title)
        {
            CommitmentsUrl = commitmentsUrl;
        }

        public string CommitmentsUrl { get; }
    }
}
