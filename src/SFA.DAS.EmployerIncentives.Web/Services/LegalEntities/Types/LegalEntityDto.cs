namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types
{
    public class LegalEntityDto
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string VrfCaseStatus { get; set; }
        public string VrfVendorId { get; set; }
        public bool IsAgreementSigned { get; set; }
        public string HashedLegalEntityId { get; set; }
    }
}
