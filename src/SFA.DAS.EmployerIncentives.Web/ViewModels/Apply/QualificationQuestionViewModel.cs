namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class QualificationQuestionViewModel : IViewModel
    {
        public const string HasTakenOnNewApprenticesNotSelectedMessage = "Select yes if you have apprentices who are eligible for the payment";

        public bool? HasTakenOnNewApprentices { get; set; }

        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }

        public string Title => $"Eligible apprentices at {OrganisationName}";

        public string OrganisationName { get; set; }
    }
}
