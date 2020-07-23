using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Collections.Generic;
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

        [Route("{accountLegalEntityId}/select-new-apprentices")]
        [HttpGet]
        public async Task<ViewResult> SelectApprenticeships(string accountId, string accountLegalEntityId)
        {
            return View(new { accountId, accountLegalEntityId } );
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply(bool hasTakenOnNewApprentices = false)
        {
            if (hasTakenOnNewApprentices)
            {
                return View(new TakenOnCannotApplyViewModel(_configuration.CommitmentsBaseUrl));
            }
            return View(new CannotApplyViewModel(_configuration.CommitmentsBaseUrl));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Default()
        {
            return RedirectToAction("GetChooseOrganisation");
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
                viewModel.Organisations = new List<OrganisationViewModel>();

                legalEntities.ToList().ForEach(o => viewModel.Organisations.Add(
                    new OrganisationViewModel { 
                        Name = o.LegalEntityName,
                        AccountLegalEntityId = o.AccountLegalEntityId 
                    }));

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

            if (string.IsNullOrEmpty(viewModel.Selected))
            {
                ModelState.AddModelError("OrganisationNotSelected", viewModel.OrganisationNotSelectedMessage);
            }

            var legalEntities = await _legalEntitiesService.Get(viewModel.AccountId);

            viewModel.Organisations = new List<OrganisationViewModel>();

            legalEntities.ToList().ForEach(o => viewModel.Organisations.Add(
                new OrganisationViewModel
                {
                    Name = o.LegalEntityName,
                    AccountLegalEntityId = o.AccountLegalEntityId
                }));

            return View(viewModel);
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously