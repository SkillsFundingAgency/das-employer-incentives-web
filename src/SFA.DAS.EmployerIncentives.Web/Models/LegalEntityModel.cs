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

        public bool BankDetailsRequired { get => MapBankDetailsRequired(VrfCaseStatus, VrfVendorId); }

        private const string RejectedDataValidation = "Case Rejected - Data validation";
        private const string RejectedVer1 = "VER1 Rejected";
        private const string RejectedVerification = "Case Rejected - Verification";

        private static bool MapBankDetailsRequired(string vrfCaseStatus, string vrfVendorId)
        {
            if (!string.IsNullOrWhiteSpace(vrfVendorId) && vrfVendorId != "000000")
            {
                return false;
            }

            return (string.IsNullOrWhiteSpace(vrfCaseStatus)
                || vrfCaseStatus.Equals(RejectedDataValidation, StringComparison.InvariantCultureIgnoreCase)
                || vrfCaseStatus.Equals(RejectedVer1, StringComparison.InvariantCultureIgnoreCase)
                || vrfCaseStatus.Equals(RejectedVerification, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}