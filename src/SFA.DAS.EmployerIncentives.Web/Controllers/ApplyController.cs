using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyController : Controller
    {
        private readonly ExternalLinksConfiguration _configuration;
        private readonly IApplicationService _applicationService;
        private readonly ILegalEntitiesService _legalEntitiesService;

        public ApplyController(
            IOptions<ExternalLinksConfiguration> configuration,
            IApplicationService applicationService,
            ILegalEntitiesService legalEntitiesService)
        {
            _configuration = configuration.Value;
            _applicationService = applicationService;
            _legalEntitiesService = legalEntitiesService;
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
            var email = ControllerContext.HttpContext.User.FindFirst(claim => claim.Type == EmployerClaimTypes.EmailAddress)?.Value;
            var firstName = ControllerContext.HttpContext.User.FindFirst(claim => claim.Type == EmployerClaimTypes.GivenName)?.Value;
            var lastName = ControllerContext.HttpContext.User.FindFirst(claim => claim.Type == EmployerClaimTypes.FamilyName)?.Value;

            await _applicationService.Confirm(accountId, applicationId, email, string.Join(" ", firstName, lastName));

            return RedirectToAction("BankDetailsConfirmation", "BankDetails", new { accountId, applicationId });            
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/no-eligible-apprentices")]
        public async Task<ViewResult> CannotApply(string accountId, string accountLegalEntityId)
        {
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);
            var title = $"{legalEntity?.Name} does not have any eligible apprentices";
            if (String.IsNullOrWhiteSpace(legalEntity?.Name)) // no legal entities associated with the account
            {
                title = $"Your organisation does not have any eligible apprentices";
            }
            return View(new CannotApplyViewModel(accountId, _configuration.ManageApprenticeshipSiteUrl, title, legalEntity?.Name));
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/cannot-apply")]
        public async Task<ViewResult> CannotApplyYet(string accountId, string accountLegalEntityId)
        {
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);
            var title = $"{legalEntity?.Name} cannot apply for this payment";
            return View(new TakenOnCannotApplyViewModel(accountId, _configuration.CommitmentsSiteUrl, title, legalEntity?.Name));
        }

        [HttpGet]
        [Route("cannot-apply-yet")]
        public async Task<IActionResult> Redirect()
        {
            return RedirectToActionPermanent("CannotApplyYet");
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously