namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class QualificationQuestionViewModel : ViewModel
    {
        public const string HasTakenOnNewApprenticesNotSelectedMessage = "Select yes if you have apprentices who are eligible for the payment";

        public QualificationQuestionViewModel() : base("Do you have apprentices who are eligible for the payment?")
        {
        }

        public bool? HasTakenOnNewApprentices { get; set; }

        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
    }
}
