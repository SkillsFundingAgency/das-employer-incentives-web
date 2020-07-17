using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class SelectApprenticesViewModel : ViewModel
    {
        public const string SelectApprenticesMessage = "Select the apprentices you want to apply for";

        public SelectApprenticesViewModel() : base(SelectApprenticesMessage)
        {
            SelectedApprentices = new List<string>();
        }

        public IEnumerable<ApprenticeModel> Apprentices { get; set; }

        public bool HasSelectedApprentices => SelectedApprentices.Count > 0;

        public string FirstCheckboxId => $"new-apprentices-{Apprentices.First().Id}";

        public List<string> SelectedApprentices { get; set; }

        public string AccountId { get; set; }
    }
}