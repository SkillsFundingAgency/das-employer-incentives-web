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
        public async Task<ViewResult> ListPayments(string accountId)
        {
            var applications = await _applicationService.GetList(accountId);

            var model = new ViewApplicationsViewModel { Applications = applications.Where(x => x.Status == "Submitted") };

            return View(model);
        }
    }
}
