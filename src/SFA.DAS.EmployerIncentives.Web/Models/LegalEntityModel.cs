namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class LegalEntityModel
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public string Name { get; set; }
        public bool HasSignedIncentiveTerms { get; set; }
        public string VrfCaseStatus { get; set; }
    }
}