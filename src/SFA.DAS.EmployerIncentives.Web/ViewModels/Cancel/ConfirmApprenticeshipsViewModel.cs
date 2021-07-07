using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Cancel
{
    public class ConfirmApprenticeshipsViewModel : IViewModel
    {
        public string Title => "Confirm apprentices";
        public string OrganisationName { get; set; }
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        public IEnumerable<ApprenticeshipIncentiveModel> ApprenticeshipIncentives { get; set; }
    }
}