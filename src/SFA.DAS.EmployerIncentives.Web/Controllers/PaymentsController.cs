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

            if (!submittedApplications.Any())
            {
                return RedirectToAction("NoApplications");
            }

            submittedApplications = SortApplications(sortOrder, sortField, submittedApplications);

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

        private static IQueryable<ApprenticeApplicationModel> SortApplications(string sortOrder, string sortField, IQueryable<ApprenticeApplicationModel> submittedApplications)
        {
            if (sortOrder == ApplicationsSortOrder.Descending)
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    submittedApplications = submittedApplications.OrderByDescending(sortField).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    submittedApplications = submittedApplications.OrderByDescending(sortField);
                }
            }
            else
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    submittedApplications = submittedApplications.OrderBy(sortField).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    submittedApplications = submittedApplications.OrderBy(sortField);
                }
            }

            return submittedApplications;
        }
    }
}
