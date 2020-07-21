namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class QualificationQuestionViewModel : ViewModel
    {
        public const string HasTakenOnNewApprenticeshipsNotSelectedMessage = "Select yes if you’ve taken on new apprenticeships that joined your payroll after 1 August 2020";

        public QualificationQuestionViewModel() : base("Have you taken on new apprenticeships that joined your payroll after 1 August 2020?")
        {
        }

        public bool? HasTakenOnNewApprentices { get; set; }
    }
}
