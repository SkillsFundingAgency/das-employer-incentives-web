using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> ListPayments(string accountId)
        {
            var applications = await _applicationService.GetList(accountId);
            var submittedApplications = applications.Where(x => x.Status == "Submitted");

            if (!submittedApplications.Any())
            {
                return RedirectToAction("NoApplications");
            }

            var model = new ViewApplicationsViewModel { Applications = submittedApplications };

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
