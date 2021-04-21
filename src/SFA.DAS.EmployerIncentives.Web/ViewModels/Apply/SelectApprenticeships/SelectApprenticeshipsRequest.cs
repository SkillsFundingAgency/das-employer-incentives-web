using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships
{
    public class SelectApprenticeshipsRequest
    {
        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }

        public List<string> SelectedApprenticeships { get; set; }

        public static string SelectedApprenticeshipsPropertyName => nameof(SelectedApprenticeships);

        public bool HasSelectedApprenticeships => SelectedApprenticeships != null && SelectedApprenticeships.Count > 0;
        public int PageNumber { get; set; }
    }
}