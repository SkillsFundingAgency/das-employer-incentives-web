namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class CannotApplyViewModel : ViewModel
    {
        public CannotApplyViewModel(string commitmentsUrl) : base("You cannot apply for this grant yet")
        {
            CommitmentsUrl = commitmentsUrl;
        }

        public string CommitmentsUrl { get; }
    }
}
