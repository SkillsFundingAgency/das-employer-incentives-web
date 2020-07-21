using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class SelectApprenticeshipsViewModel : ViewModel
    {
        public const string SelectApprenticeshipsMessage = "Select the apprentices you want to apply for";

        public SelectApprenticeshipsViewModel() : base(SelectApprenticeshipsMessage)
        {
            SelectedApprenticeships = new List<string>();
        }

        public IEnumerable<ApprenticeshipModel> Apprenticeships { get; set; }

        public bool HasSelectedApprenticeships => SelectedApprenticeships.Count > 0;

        public string FirstCheckboxId => $"new-apprenticeships-{Apprenticeships.First().Id}";

        public List<string> SelectedApprenticeships { get; set; }

        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }
    }
}