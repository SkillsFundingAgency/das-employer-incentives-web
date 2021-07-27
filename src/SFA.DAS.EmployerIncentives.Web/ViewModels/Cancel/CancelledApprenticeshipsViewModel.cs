using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Cancel
{
    public class CancelledApprenticeshipsViewModel : IViewModel
    {
        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }

        public IEnumerable<ApprenticeshipIncentiveModel> ApprenticeshipIncentives { get; set; }

        public string Title => "Application cancelled";

        public string OrganisationName { get; set; }
        public string ViewApplicationsUrl => $"/{AccountId}/payments/{AccountLegalEntityId}/payment-applications";
    }
}