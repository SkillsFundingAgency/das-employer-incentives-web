using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
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
        private readonly IHashingService _hashingService;

        public ApplyController(
            IOptions<ExternalLinksConfiguration> configuration,
            IApplicationService applicationService,
            ILegalEntitiesService legalEntitiesService,
            IHashingService hashingService)
        {
            _configuration = configuration.Value;
            _applicationService = applicationService;
            _legalEntitiesService = legalEntitiesService;
            _hashingService = hashingService;
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

            var accountLegalEntityId = await _applicationService.GetApplicationLegalEntity(accountId, applicationId);

            var legalEntity = await _legalEntitiesService.Get(accountId, _hashingService.HashValue(accountLegalEntityId));

            if (String.IsNullOrEmpty(legalEntity.VrfCaseStatus))
            {
                return RedirectToAction("BankDetailsConfirmation", "BankDetails", new { accountId, applicationId });
            }
            else
            {
                return RedirectToAction("Confirmation", "ApplicationComplete");
            }
            
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply(string accountId)
        {
            return View(new CannotApplyViewModel(accountId, _configuration.ManageApprenticeshipSiteUrl));
        }

        [HttpGet]
        [Route("cannot-apply-yet")]
        public async Task<ViewResult> CannotApplyYet(string accountId)
        {
            return View(new TakenOnCannotApplyViewModel(accountId, _configuration.CommitmentsSiteUrl));
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously