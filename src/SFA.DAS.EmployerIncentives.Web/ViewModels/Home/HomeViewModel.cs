using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Home
{
    public class HomeViewModel : IViewModel
    {
        public string Title => "Apply for the hire a new apprentice payment";

        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public bool NewAgreementRequired { get; }
        public bool BankDetailsRequired { get; }        
        public string AccountsAgreementsUrl { get; }
        public string OrganisationName { get; set; }

        private string ManageApprenticeshipSiteUrl { get; }
        private const string RejectedDataValidation = "Case Rejected - Data validation";
        private const string RejectedVer1 = "VER1 Rejected";
        private const string RejectedVerification = "Case Rejected - Verification";

        public HomeViewModel(
            string accountId, 
            string accountLegalEntityId, 
            string organisationName, 
            bool hasMultipleLegalEntities, 
            bool newAgreementRequired, 
            string manageApprenticeshipSiteUrl,
            string vrfCaseStatus,
            string vrfVendorId)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            OrganisationName = organisationName;
            NewAgreementRequired = newAgreementRequired;
            ManageApprenticeshipSiteUrl = manageApprenticeshipSiteUrl;
            if (!manageApprenticeshipSiteUrl.EndsWith("/"))
            {
                ManageApprenticeshipSiteUrl += "/";
            }
            AccountsAgreementsUrl = $"{manageApprenticeshipSiteUrl}accounts/{AccountId}/teams";
            BankDetailsRequired = MapBankDetailsRequired(vrfCaseStatus, vrfVendorId);
        }

        // TODO: move to api
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
