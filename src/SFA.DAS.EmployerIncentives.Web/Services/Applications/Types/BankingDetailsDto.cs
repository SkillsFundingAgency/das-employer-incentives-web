using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class BankingDetailsDto
    {
        public long LegalEntityId { get; set; }
        public string VendorCode { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public decimal ApplicationValue { get; set; }
        public IEnumerable<SignedAgreementDto> SignedAgreements { get; set; }
    }
}