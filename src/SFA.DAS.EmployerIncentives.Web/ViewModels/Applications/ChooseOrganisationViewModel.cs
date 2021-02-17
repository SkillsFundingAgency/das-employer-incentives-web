using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Applications
{
    public class ChooseOrganisationViewModel : IViewModel
    {
        public string LegalEntityNotSelectedMessage => "Select an organisation";

        public ChooseOrganisationViewModel()
        {

        }
        public ChooseOrganisationViewModel(string accountsBaseUrl, string accountId)
        {
            AccountId = accountId;
            if (!accountsBaseUrl.EndsWith("/"))
            {
                accountsBaseUrl += "/";
            }
            AccountHomeUrl = $"{accountsBaseUrl}accounts/{AccountId}/teams";
        }

        public string AccountId { get; set; }
        public IEnumerable<LegalEntityModel> LegalEntities { get; set; }

        public string Selected { get; set; }

        public string AccountHomeUrl { get; set; }

        public string Title => "Choose organisation";

        public string OrganisationName { get; set; }
    }
}
