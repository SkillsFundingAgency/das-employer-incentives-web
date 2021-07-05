using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Cancel
{
    public class SelectApprenticeshipsViewModel : IViewModel
    {
        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }

        public IEnumerable<ApprenticeshipIncentiveModel> ApprenticeshipIncentives { get; set; }

        public string FirstCheckboxId => $"cancel-apprenticeships-{ApprenticeshipIncentives.FirstOrDefault()?.Id}";

        public string Title => "Which apprentices do you want to cancel an application for?";

        public string OrganisationName { get; set; }

        public string SelectApprenticeshipsMessage => "Select which apprentices you want to cancel an application for";
    }
}