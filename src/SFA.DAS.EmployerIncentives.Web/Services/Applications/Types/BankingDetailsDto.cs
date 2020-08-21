using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class BankingDetailsDto
    {
        public long LegalEntityId { get; set; }
        public string VendorCode { get; set; }
        public string SubmittedByName { get; set; }
        public string SubmittedByEmail { get; set; }
        public decimal ApplicationValue { get; set; }
        public IEnumerable<SignedAgreementDto> SignedAgreements { get; set; }
        public int NumberOfApprenticeships { get; set; }
    }
}