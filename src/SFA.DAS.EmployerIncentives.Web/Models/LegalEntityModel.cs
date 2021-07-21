using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class LegalEntityModel
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public string Name { get; set; }
        public string VrfCaseStatus { get; set; }
        public string VrfVendorId { get; set; }
        public string HashedLegalEntityId { get; set; }
        public bool IsAgreementSigned { get; set; }
        public bool BankDetailsRequired { get; set; }
    }
}