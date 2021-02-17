
using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Hub
{
    public class HubPageViewModel : IViewModel
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public string OrganisationName { get; set; }
        public bool HasMultipleLegalEntities { get; set; }
        public string AccountHomeUrl { get; set; }
        public bool ShowBankDetailsRequired { get; set; }
        public Guid BankDetailsApplicationId { get; set; }

        public string Title => "Hire a new apprentice payment";

        public HubPageViewModel(string accountsBaseUrl, string accountId)
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
