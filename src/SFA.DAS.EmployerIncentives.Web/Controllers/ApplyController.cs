using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyController : Controller
    {
        private readonly WebConfigurationOptions _configuration;
        private readonly IApplicationService _applicationService;

        public ApplyController(
            IOptions<WebConfigurationOptions> configuration,
            IApplicationService applicationService)
        {
            _configuration = configuration.Value;
            _applicationService = applicationService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Default()
        {
            return RedirectToAction("GetChooseOrganisation", "ApplyOrganisation");
        }

        [HttpGet]
        [Route("declaration/{applicationId}")]
        public async Task<ViewResult> Declaration(string accountId, Guid applicationId)
        {
            return View(new DeclarationViewModel(accountId, applicationId));
        }

        [HttpPost]
        [Route("declaration/{applicationId}")]
        public async Task<IActionResult> SubmitApplication(string accountId, Guid applicationId)
        {
            var userId = ControllerContext.HttpContext.User.FindFirst(c => c.Type.Equals(EmployerClaimTypes.UserId))?.Value;

            await _applicationService.Confirm(accountId, applicationId, userId);

            return RedirectToAction("BankDetailsConfirmation", "BankDetails", new { accountId, applicationId });
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply(string accountId)
        {
            return View(new CannotApplyViewModel(accountId, _configuration.AccountsBaseUrl));
        }

        [HttpGet]
        [Route("cannot-apply-yet")]
        public async Task<ViewResult> CannotApplyYet(string accountId)
        {
            return View(new TakenOnCannotApplyViewModel(accountId, _configuration.CommitmentsBaseUrl));
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously