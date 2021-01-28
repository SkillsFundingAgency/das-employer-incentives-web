
namespace SFA.DAS.EmployerIncentives.Web.ViewModels
{
    public class HubPageViewModel
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public string OrganisationName { get; set; }
        public bool HasMultipleLegalEntities { get; set; }
        public string AccountHomeUrl { get; set; }

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
