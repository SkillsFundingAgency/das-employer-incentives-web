using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Threading.Tasks;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.EmployerIncentives.Web.Exceptions;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    [SetNavigationSection(NavigationSection.AccountsFinance)]
    public class ApplyController : ControllerBase
    {
        private readonly ExternalLinksConfiguration _configuration;
        private readonly IApplicationService _applicationService;

        public ApplyController(
            IOptions<ExternalLinksConfiguration> configuration,
            IApplicationService applicationService,
            ILegalEntitiesService legalEntitiesService) : base(legalEntitiesService)
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
        public async Task<IActionResult> Declaration(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);
            if(application == null)
            {
                return RedirectToAction("Home", "Home", new { accountId });
            }
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            return View(new DeclarationViewModel(accountId, applicationId, legalEntityName, _configuration.ManageApprenticeshipSiteUrl));
        }

        [HttpPost]
        [Route("declaration/{applicationId}")]
        public async Task<IActionResult> SubmitApplication(string accountId, Guid applicationId)
        {
            var email = ControllerContext.HttpContext.User.FindFirst(claim => claim.Type == EmployerClaimTypes.EmailAddress)?.Value;
            var firstName = ControllerContext.HttpContext.User.FindFirst(claim => claim.Type == EmployerClaimTypes.GivenName)?.Value;
            var lastName = ControllerContext.HttpContext.User.FindFirst(claim => claim.Type == EmployerClaimTypes.FamilyName)?.Value;

            try
            {
                await _applicationService.Confirm(accountId, applicationId, email, string.Join(" ", firstName, lastName));
            }
            catch (UlnAlreadySubmittedException)
            {
                var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false, includeSubmitted: true);
                return RedirectToAction("UlnAlreadyAppliedFor", new {accountId, application.AccountLegalEntityId });
            }

            return RedirectToAction("BankDetailsConfirmation", "BankDetails", new { accountId, applicationId });            
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/no-new-apprentices")]
        public async Task<ViewResult> CannotApply(string accountId, string accountLegalEntityId)
        {
            var legalEntityName = await GetLegalEntityName(accountId, accountLegalEntityId);
            var model = new TakenOnCannotApplyViewModel(accountId, _configuration.CommitmentsSiteUrl, _configuration.ManageApprenticeshipSiteUrl, legalEntityName);
            return View(model);
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<IActionResult> Forbidden()
        {
            return Forbid();
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/cannot-apply")]
        public async Task<ViewResult> CannotApplyYet(string accountId, string accountLegalEntityId)
        {
            var legalEntityName = await GetLegalEntityName(accountId, accountLegalEntityId);
            var model = new CannotApplyViewModel(accountId, _configuration.ManageApprenticeshipSiteUrl, legalEntityName);
            return View(model);
        }

        [HttpGet]
        [Route("cannot-apply-yet")]
        public async Task<IActionResult> Redirect()
        {
            return RedirectToActionPermanent("CannotApplyYet");
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/no-eligible-apprentices")]
        public async Task<IActionResult> RedirectShutter()
        {
            return RedirectToActionPermanent("CannotApply");
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/problem-with-service")]
        public async Task<IActionResult> UlnAlreadyAppliedFor(string accountId, string accountLegalEntityId)
        {
            var viewModel = new UlnAlreadyAppliedForViewModel(accountId, accountLegalEntityId);
            return View(viewModel);
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously