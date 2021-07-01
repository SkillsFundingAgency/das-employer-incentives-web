using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class BankDetailsConfirmationViewModel : IViewModel
    {
        public string CanProvideBankDetailsNotSelectedMessage => $"Select yes if you can add {OrganisationName}'s organisation and finance details now";

        public string AccountId { get; set; }
        public Guid ApplicationId { get; set; }
        public bool? CanProvideBankDetails { get; set; }

        public string Title => $"Can you add {OrganisationName}'s organisation and finance details now?";

        public string OrganisationName { get; set; }
    }
}
