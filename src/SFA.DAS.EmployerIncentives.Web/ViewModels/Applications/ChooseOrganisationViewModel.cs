
using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Applications
{
    public class ChooseOrganisationViewModel : ViewModel
    {
        public string LegalEntityNotSelectedMessage => "Select an organisation";

        public ChooseOrganisationViewModel(string accountsBaseUrl, string accountId) : base("Choose organisation")
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

    }
}
