namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class TakenOnCannotApplyViewModel : CannotApplyViewModel
    {
        public TakenOnCannotApplyViewModel(string commitmentsUrl, string title = "You cannot apply for this grant") : base(commitmentsUrl, title)
        {
        }
    }
}
