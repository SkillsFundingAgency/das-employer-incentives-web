namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types
{
    public class LegalEntityDto
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public bool HasSignedIncentiveTerms { get; set; }
    }
}
