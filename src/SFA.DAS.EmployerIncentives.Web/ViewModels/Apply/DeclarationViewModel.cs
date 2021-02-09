using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class DeclarationViewModel : IViewModel
    {
        public string AccountId { get; }
        public Guid ApplicationId { get; }

        public string Title => "Declaration";

        public string OrganisationName { get; set; }

        public DeclarationViewModel(string accountId, Guid applicationId)
        {
            AccountId = accountId;
            ApplicationId = applicationId;
        }
    }
}