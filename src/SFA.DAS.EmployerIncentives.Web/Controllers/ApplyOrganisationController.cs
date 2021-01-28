using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyOrganisationController : Controller
    {
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly ExternalLinksConfiguration _configuration;

        public ApplyOrganisationController(ILegalEntitiesService legalEntitiesService, IOptions<ExternalLinksConfiguration> configuration)
        {
            _legalEntitiesService = legalEntitiesService;
            _configuration = configuration.Value;
        }

        [HttpGet]
        [Route("choose-organisation")]
        public async Task<IActionResult> GetChooseOrganisation(ChooseOrganisationViewModel viewModel)
        {
            var legalEntities = await _legalEntitiesService.Get(viewModel.AccountId);
            if (legalEntities.Count() == 1)
            {
                return RedirectToAction("Index", "Hub", new { viewModel.AccountId, accountLegalEntityId = legalEntities.First().AccountLegalEntityId });
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
                return RedirectToAction("Index", "Hub", new { viewModel.AccountId, accountLegalEntityId = viewModel.Selected });
            }

            viewModel.AddOrganisations(await _legalEntitiesService.Get(viewModel.AccountId));

            if (string.IsNullOrEmpty(viewModel.Selected))
            {
                ModelState.AddModelError(viewModel.Organisations.Any() ? viewModel.Organisations.First().AccountLegalEntityId : "OrganisationNotSelected", viewModel.OrganisationNotSelectedMessage);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/validate-terms-signed")]
        public async Task<IActionResult> ValidateTermsSigned(string accountId, string accountLegalEntityId)
        {
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);

            if (legalEntity.HasSignedIncentiveTerms)
            {
                return RedirectToAction("SelectApprenticeships", "ApplyApprenticeships", new { accountId, accountLegalEntityId });
            }

            var viewModel = new ValidateTermsSignedViewModel(accountId, _configuration.ManageApprenticeshipSiteUrl);
            return View(viewModel);
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously