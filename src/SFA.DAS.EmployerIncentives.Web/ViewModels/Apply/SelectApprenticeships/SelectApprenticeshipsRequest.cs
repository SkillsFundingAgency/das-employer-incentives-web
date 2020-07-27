using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships
{
    public class SelectApprenticeshipsRequest : ISelectedApprenticeships
    {
        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }

        public List<string> SelectedApprenticeships { get; set; } = new List<string>();

        public bool HasSelectedApprenticeships => SelectedApprenticeships.Count > 0;
    }
}