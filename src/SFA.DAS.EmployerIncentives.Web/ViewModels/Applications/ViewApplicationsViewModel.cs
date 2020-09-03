
using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Applications
{
    public class ViewApplicationsViewModel : ViewModel
    {
        public ViewApplicationsViewModel() : base ("Your hire a new apprentice payment applications")
        {
            
        }

        public IEnumerable<ApprenticeApplicationModel> Applications { get; set; }
    }
}
