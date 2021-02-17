using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{    
    public class ChooseOrganisationViewModel : IViewModel
    {
        public string OrganisationNotSelectedMessage => "Select an organisation";
       
        public string AccountId { get; set; }
        public List<OrganisationViewModel> Organisations { get; set; }

        public string Selected { get; set; }

        public string AccountHomeUrl { get; set; }

        public string Title => "Choose organisation";

        public string OrganisationName { get; set; }

        public void SetManageAccountsUrl(string accountsBaseUrl)
        {
            if (!accountsBaseUrl.EndsWith("/"))
            {
                accountsBaseUrl += "/";
            }
            AccountHomeUrl = $"{accountsBaseUrl}accounts/{AccountId}/teams";
        }
    }
}
