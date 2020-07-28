using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships
{
    public class SelectApprenticeshipsViewModel : ViewModel
    {
        public const string SelectApprenticeshipsMessage = "Select the apprentices you want to apply for";

        public SelectApprenticeshipsViewModel() : base(SelectApprenticeshipsMessage) { }

        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }

        public IEnumerable<ApprenticeshipModel> Apprenticeships { get; set; }

        public string FirstCheckboxId => $"new-apprenticeships-{Apprenticeships.FirstOrDefault()?.Id}";

    }
}