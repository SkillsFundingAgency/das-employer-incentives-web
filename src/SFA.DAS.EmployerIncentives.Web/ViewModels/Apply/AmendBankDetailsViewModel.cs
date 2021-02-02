using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class AmendBankDetailsViewModel : ViewModel
    {
        public string AccountId { get; set; }
        public Guid ApplicationId { get; set; }
        public string OrganisationName { get; set; }

        public AmendBankDetailsViewModel(string title, string accountId, Guid applicationId, string organisationName) : base(title)
        {
            AccountId = accountId;
            ApplicationId = applicationId;
            OrganisationName = organisationName;
        }
    }
}
