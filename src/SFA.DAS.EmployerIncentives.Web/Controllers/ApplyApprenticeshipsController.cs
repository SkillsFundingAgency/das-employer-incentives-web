using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyApprenticeshipsController : Controller
    {
        private readonly IApprenticesService _apprenticesService;
        private readonly IApplicationService _applicationService;
        private readonly ILegalEntitiesService _legalEntityService;
        private readonly ExternalLinksConfiguration _configuration;

        public ApplyApprenticeshipsController(
            IApprenticesService apprenticesService,
            IApplicationService applicationService,
            ILegalEntitiesService legalEntityService,
            IOptions<ExternalLinksConfiguration> configuration)
        {
            _apprenticesService = apprenticesService;
            _applicationService = applicationService;
            _legalEntityService = legalEntityService;
            _configuration = configuration.Value;
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/select-apprentices")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, string accountLegalEntityId)
        {
            var model = await GetInitialSelectApprenticeshipsViewModel(accountId, accountLegalEntityId);

            if (!model.Apprenticeships.Any())
            {
                return RedirectToAction("CannotApplyYet", "Apply", new { accountId, accountLegalEntityId });
            }

            return View(model);
        }

        [HttpPost]
        [Route("{accountLegalEntityId}/select-apprentices")]
        public async Task<IActionResult> SelectApprenticeships(SelectApprenticeshipsRequest form)
        {
            if (form.HasSelectedApprenticeships)
            {
                var applicationId = await _applicationService.Create(form.AccountId, form.AccountLegalEntityId, form.SelectedApprenticeships);
                return RedirectToAction("ConfirmApprenticeships", new { form.AccountId, applicationId });
            }

            var viewModel = await GetInitialSelectApprenticeshipsViewModel(form.AccountId, form.AccountLegalEntityId);
            ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);

            return View(viewModel);
        }

        [HttpGet]
        [Route("select-apprentices/{applicationId}")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, Guid applicationId)
        {
            var model = await GetSelectApprenticeshipsViewModel(accountId, applicationId);

            if (!model.Apprenticeships.Any())
            {
                return RedirectToAction("CannotApplyYet", "Apply", new { accountId, accountLegalEntityId = model.AccountLegalEntityId });
            }

            return View(model);
        }

        [HttpPost]
        [Route("select-apprentices/{applicationId}")]
        public async Task<IActionResult> SelectApprenticeships(SelectApprenticeshipsRequest form, Guid applicationId)
        {
            if (form.HasSelectedApprenticeships)
            {
                await _applicationService.Update(applicationId, form.AccountId, form.SelectedApprenticeships);
                return RedirectToAction("ConfirmApprenticeships", new { form.AccountId, applicationId });
            }

            var viewModel = await GetSelectApprenticeshipsViewModel(form.AccountId, applicationId, false);
            ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);

            return View(viewModel);
        }

        [HttpGet]
        [Route("confirm-apprentices/{applicationId}")]
        public async Task<IActionResult> ConfirmApprenticeships(string accountId, Guid applicationId)
        {
            var model = await _applicationService.Get(accountId, applicationId);
            return View(model);
        }

        [HttpPost]
        [Route("confirm-apprentices/{applicationId}")]
        public async Task<IActionResult> DisplayDeclaration(string accountId, Guid applicationId, bool newAgreementRequired)
        {
            if(newAgreementRequired)
                return RedirectToAction("NewAgreementRequired", new { accountId, applicationId });

            return RedirectToAction("Declaration", "Apply", new { accountId, applicationId });
        }

        [HttpGet]
        [Route("accept-new-agreement/{applicationId}")]
        public async Task<IActionResult> NewAgreementRequired(string accountId, Guid applicationId)
        {
            var accountLegalEntityId = await _applicationService.GetApplicationLegalEntity(accountId, applicationId);
            var legalEntity = await _legalEntityService.Get(accountId, accountLegalEntityId);
            var viewModel = new NewAgreementRequiredViewModel(legalEntity.Name, accountId, applicationId, _configuration.ManageApprenticeshipSiteUrl);
            return View(viewModel);
        }

        private async Task<SelectApprenticeshipsViewModel> GetInitialSelectApprenticeshipsViewModel(string accountId, string accountLegalEntityId)
        {
            var apprenticeships = await _apprenticesService.Get(new ApprenticesQuery(accountId, accountLegalEntityId));

            return new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                Apprenticeships = apprenticeships.OrderBy(a => a.LastName)
            };
        }

        private async Task<SelectApprenticeshipsViewModel> GetSelectApprenticeshipsViewModel(string accountId, Guid applicationId, bool showSelected = true)
        {
            var application = await _applicationService.Get(accountId, applicationId);

            var apprenticeships = (await _apprenticesService.Get(new ApprenticesQuery(accountId, application.AccountLegalEntityId))).ToList();

            if (showSelected)
            {
                apprenticeships.ForEach(x => x.Selected = application.Apprentices.Any(a => a.ApprenticeshipId == x.Id));
            }

            return new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = application.AccountLegalEntityId,
                Apprenticeships = apprenticeships.OrderBy(a => a.LastName)
            };
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously