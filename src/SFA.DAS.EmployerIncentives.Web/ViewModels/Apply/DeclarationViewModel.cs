using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class DeclarationViewModel : ViewModel
    {
        public string AccountId { get; }
        public Guid ApplicationId { get; }

        public DeclarationViewModel(string accountId, Guid applicationId) : base("Declaration")
        {
            AccountId = accountId;
            ApplicationId = applicationId;
        }
    }
}