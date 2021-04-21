using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships
{
    public class SelectApprenticeshipsViewModel : IViewModel
    {
        public const string SelectApprenticeshipsMessage = "Select which apprentices you want to apply for";

        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }

        public IEnumerable<ApprenticeshipModel> Apprenticeships { get; set; }

        public string FirstCheckboxId => $"new-apprenticeships-{Apprenticeships.FirstOrDefault()?.Id}";

        public string Title => "Which apprentices do you want to apply for?";

        public string OrganisationName { get; set; }
        public int PageNumber { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public bool PrevPage { get; set; }
        public bool NextPage { get; set; }
    }
}