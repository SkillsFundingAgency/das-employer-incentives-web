using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyOrganisationController : Controller
    {
        private readonly ILegalEntitiesService _legalEntitiesService;

        public ApplyOrganisationController(ILegalEntitiesService legalEntitiesService)
        {
            _legalEntitiesService = legalEntitiesService;
        }

        [HttpGet]
        [Route("choose-organisation")]
        public async Task<IActionResult> GetChooseOrganisation(ChooseOrganisationViewModel viewModel)
        {
            var legalEntities = await _legalEntitiesService.Get(viewModel.AccountId);
            if (legalEntities.Count() == 1)
            {
                return RedirectToAction("GetQualificationQuestion", "ApplyQualification", new { viewModel.AccountId, accountLegalEntityId = legalEntities.First().AccountLegalEntityId });
            }
            if (legalEntities.Count() > 1)
            {
                viewModel.AddOrganisations(legalEntities);
                return View("ChooseOrganisation", viewModel);
            }

            return RedirectToAction("CannotApply", "Apply",  new { viewModel.AccountId});
        }

        [HttpPost]
        [Route("choose-organisation")]
        public async Task<IActionResult> ChooseOrganisation(ChooseOrganisationViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Selected))
            {
                return RedirectToAction("QualificationQuestion", "ApplyQualification", new { viewModel.AccountId, accountLegalEntityId = viewModel.Selected });
            }

            viewModel.AddOrganisations(await _legalEntitiesService.Get(viewModel.AccountId));

            if (string.IsNullOrEmpty(viewModel.Selected))
            {
                ModelState.AddModelError(viewModel.Organisations.Any() ? viewModel.Organisations.First().AccountLegalEntityId : "OrganisationNotSelected", viewModel.OrganisationNotSelectedMessage);
            }

            return View(viewModel);
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously