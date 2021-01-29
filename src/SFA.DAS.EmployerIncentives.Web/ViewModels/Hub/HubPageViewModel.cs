
using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Hub
{
    public class HubPageViewModel : ViewModel
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public string OrganisationName { get; set; }
        public bool HasMultipleLegalEntities { get; set; }
        public string AccountHomeUrl { get; set; }
        public bool ShowBankDetailsRequired { get; set; }
        public Guid BankDetailsApplicationId { get; set; }

        public HubPageViewModel(string accountsBaseUrl, string accountId, string title = "Hire a new apprentice payment") : base(title)
        {
            AccountId = accountId;
            if (!accountsBaseUrl.EndsWith("/"))
            {
                accountsBaseUrl += "/";
            }
            AccountHomeUrl = $"{accountsBaseUrl}accounts/{AccountId}/teams";
        }
    }
}
