namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class QualificationQuestionViewModel : ViewModel
    {
        public QualificationQuestionViewModel() : base("Have you taken on new apprentices that joined your payroll after 1 August 2020?")
        {
        }

        public bool? HasTakenOnNewApprentices { get; set; }
    }
}
