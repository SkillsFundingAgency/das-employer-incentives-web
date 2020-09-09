using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using SFA.DAS.EmployerIncentives.Web.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/payments")]
    public class PaymentsController : Controller
    {
        private readonly IApplicationService _applicationService;

        public PaymentsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [Route("payment-applications")]
        public async Task<IActionResult> ListPayments(string accountId, string sortOrder, string sortField)
        {
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                sortOrder = ApplicationsSortOrder.Ascending;
            }
            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = ApplicationsSortField.ApprenticeName;
            }

            var applications = await _applicationService.GetList(accountId);
            var submittedApplications = applications.Where(x => x.Status == "Submitted").AsQueryable();
            if (sortOrder == ApplicationsSortOrder.Descending)
            {
                submittedApplications = submittedApplications.OrderByDescending(sortField);
            }
            else
            {
                submittedApplications = submittedApplications.OrderBy(sortField);
            }

            if (!submittedApplications.Any())
            {
                return RedirectToAction("NoApplications");
            }

            var model = new ViewApplicationsViewModel
            {
                Applications = submittedApplications,
                SortField = sortField
            };
            model.SetSortOrder(sortField, sortOrder);

            return View(model);
        }

        [Route("no-applications")]
        public ViewResult NoApplications()
        {
            var model = new NoApplicationsViewModel();
            return View(model);
        }
    }
}
