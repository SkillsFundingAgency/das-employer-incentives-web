
using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class BankDetailsConfirmationViewModel : ViewModel
    {
        public const string CanProvideBankDetailsNotSelectedMessage = "Select yes if you can add your organisation's bank details now";

        public BankDetailsConfirmationViewModel() : base ("We need your organisation's bank details")
        {
        }

        public Guid ApplicationId { get; set; }
        public bool? CanProvideBankDetails { get; set; }
    }
}
