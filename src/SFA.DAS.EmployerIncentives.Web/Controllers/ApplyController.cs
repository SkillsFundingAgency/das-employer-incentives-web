using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyController : Controller
    {
        private readonly WebConfigurationOptions _configuration;
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly IApprenticesService _apprenticesService;        

        public ApplyController(
            IOptions<WebConfigurationOptions> configuration,
            ILegalEntitiesService legalEntitiesService,
            IApprenticesService apprenticesService)
        {
            _configuration = configuration.Value;
            _legalEntitiesService = legalEntitiesService;
            _apprenticesService = apprenticesService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Default()
        {
            return RedirectToAction("GetChooseOrganisation");
        }

        [HttpGet]
        [Route("declaration")]
        public async Task<ViewResult> Declaration(string accountId)
        {
            return View(new DeclarationViewModel(accountId));
        }

        [Route("{accountLegalEntityId}/taken-on-new-apprentices")]
        [HttpGet]
        public async Task<IActionResult> GetQualificationQuestion(QualificationQuestionViewModel viewModel)
        {
            return View("QualificationQuestion", viewModel);
        }

        [Route("{accountLegalEntityId}/taken-on-new-apprentices")]
        [HttpPost]
        public async Task<IActionResult> QualificationQuestion(QualificationQuestionViewModel viewModel)
        {
            if (!viewModel.HasTakenOnNewApprentices.HasValue)
            {
                ModelState.AddModelError("HasTakenOnNewApprentices", QualificationQuestionViewModel.HasTakenOnNewApprenticesNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.HasTakenOnNewApprentices.Value)
            {
                return RedirectToAction("SelectApprenticeships", new { viewModel.AccountId, accountLegalEntityId = viewModel.AccountLegalEntityId });
            }

            return RedirectToAction("CannotApply", new { viewModel.AccountId });
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply(string accountId, bool hasTakenOnNewApprentices = false)
        {
            if (hasTakenOnNewApprentices)
            {
                return View(new TakenOnCannotApplyViewModel(accountId, _configuration.CommitmentsBaseUrl));
            }
            return View(new CannotApplyViewModel(accountId, _configuration.CommitmentsBaseUrl));
        }        

        [HttpGet]
        [Route("choose-organisation")]
        public async Task<IActionResult> GetChooseOrganisation(ChooseOrganisationViewModel viewModel)
        {
            var legalEntities = await _legalEntitiesService.Get(viewModel.AccountId);
            if (legalEntities.Count() == 1)
            {
                var accountLegalEntityId = legalEntities.First().AccountLegalEntityId;
                var apprentices = await _apprenticesService.Get(new ApprenticesQuery(viewModel.AccountId, accountLegalEntityId));
                if (apprentices.Any())
                {
                    return RedirectToAction("GetQualificationQuestion", new { viewModel.AccountId, accountLegalEntityId });
                }
                else
                {
                    return RedirectToAction("CannotApply", new { viewModel.AccountId, hasTakenOnNewApprentices = true });
                }
            }
            if (legalEntities.Count() > 1)
            {
                viewModel.AddOrganisations(legalEntities);
                return View("ChooseOrganisation", viewModel);
            }

            return RedirectToAction("CannotApply", new { viewModel.AccountId });            
        }

        [HttpPost]
        [Route("choose-organisation")]
        public async Task<IActionResult> ChooseOrganisation(ChooseOrganisationViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Selected))
            {
                return RedirectToAction("GetQualificationQuestion", new { viewModel.AccountId, accountLegalEntityId = viewModel.Selected });
            }

            viewModel.AddOrganisations(await _legalEntitiesService.Get(viewModel.AccountId));

            if (string.IsNullOrEmpty(viewModel.Selected))
            {
                ModelState.AddModelError(viewModel.Organisations.Any() ? viewModel.Organisations.First().AccountLegalEntityId : "OrganisationNotSelected", viewModel.OrganisationNotSelectedMessage);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/select-new-apprentices")]
        public async Task<ViewResult> SelectApprenticeships(string accountId, string accountLegalEntityId)
        {
            var model = await GetInitialSelectApprenticeshipsViewModel(accountId, accountLegalEntityId);

            return View(model);
        }

        [HttpPost]
        [Route("{accountLegalEntityId}/select-new-apprentices")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, string accountLegalEntityId, [FromBody] SelectApprenticeshipsViewModel viewModel)
        {
            if (viewModel.HasSelectedApprenticeships)
            {
                return RedirectToAction("Declaration", new { accountId });
            }

            ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
            return View(viewModel);
        }

        private async Task<SelectApprenticeshipsViewModel> GetInitialSelectApprenticeshipsViewModel(string accountId, string accountLegalEntityId)
        {
            var apprenticeships = await _apprenticesService.Get(new ApprenticesQuery(accountId, accountLegalEntityId));

            var model = new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                Apprenticeships = apprenticeships.OrderBy(a => a.LastName)
            };
            return model;
        }          
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously