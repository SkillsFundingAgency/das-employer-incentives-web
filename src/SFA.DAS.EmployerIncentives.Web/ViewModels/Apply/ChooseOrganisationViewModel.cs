using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{    
    public class ChooseOrganisationViewModel : ViewModel
    {
        public string OrganisationNotSelectedMessage => "Select an organisation";
        public ChooseOrganisationViewModel() : base("Choose organisation")
        {
        }

        public string AccountId { get; set; }
        public List<OrganisationViewModel> Organisations { get; set; }

        public string Selected { get; set; }
    }
}
