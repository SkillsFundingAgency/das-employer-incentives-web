using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class BankDetailsConfirmationViewModel : IViewModel
    {
        public const string CanProvideBankDetailsNotSelectedMessage = "Select yes if you can add your organisation's bank details now";

        public string AccountId { get; set; }
        public Guid ApplicationId { get; set; }
        public bool? CanProvideBankDetails { get; set; }

        public string Title => "We need your organisation's bank details";

        public string OrganisationName { get; set; }
    }
}
