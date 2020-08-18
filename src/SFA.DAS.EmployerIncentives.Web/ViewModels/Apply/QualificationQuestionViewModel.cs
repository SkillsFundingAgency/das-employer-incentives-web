namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class QualificationQuestionViewModel : ViewModel
    {
        public const string HasTakenOnNewApprenticesNotSelectedMessage = "Select yes if you've taken on new apprentices who started their contract of employment between 1 August 2020 and 31 January 2021";

        public QualificationQuestionViewModel() : base("Have you taken on new apprentices who started their contract of employment between 1 August 2020 and 31 January 2021?")
        {
        }

        public bool? HasTakenOnNewApprentices { get; set; }

        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
    }
}
