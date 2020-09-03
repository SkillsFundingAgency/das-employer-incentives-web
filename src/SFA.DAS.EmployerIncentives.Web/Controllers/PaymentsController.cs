using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using System;
using System.Collections.Generic;
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
            //var applications = await _applicationService.GetList(accountId);
            //
            // FOR TESTING
            var applications = new List<ApprenticeApplicationModel>
            {
                new ApprenticeApplicationModel { ApplicationDate = new DateTime(2020,08,02), AccountId = 123, FirstName = "Fred", LastName = "Smith", LegalEntityName = "Freds Firm", TotalIncentiveAmount = 1500},
                new ApprenticeApplicationModel { ApplicationDate = new DateTime(2020,08,02), AccountId = 123, FirstName = "Steve", LastName = "Jones", LegalEntityName = "Boots", TotalIncentiveAmount = 2000},
                new ApprenticeApplicationModel { ApplicationDate = new DateTime(2020,08,02), AccountId = 123, FirstName = "Jane", LastName = "Jennings", LegalEntityName = "Company With A Long Name That Takes Up Space", TotalIncentiveAmount = 1500},
            };
            // FOR UI TESTING

            var model = new ViewApplicationsViewModel { Applications = applications };

            return View(model);
        }
    }
}
