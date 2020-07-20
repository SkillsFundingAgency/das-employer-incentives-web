using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyController : Controller
    {
        private readonly EmployerIncentivesWebConfiguration _configuration;
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly IApprenticesService _apprenticesService;
        private readonly IHashingService _hashingService;

        public ApplyController(
            IOptions<EmployerIncentivesWebConfiguration> configuration,
            ILegalEntitiesService legalEntitiesService,
            IApprenticesService apprenticesService,
            IHashingService hashingService)
        {
            _configuration = configuration.Value;
            _legalEntitiesService = legalEntitiesService;
            _apprenticesService = apprenticesService;
            _hashingService = hashingService;
        }

        [Route("")]
        [HttpGet]
        public async Task<ViewResult> QualificationQuestion()
        {
            return View(new QualificationQuestionViewModel());
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> QualificationQuestion(string accountId, QualificationQuestionViewModel viewModel)
        {
            if (!viewModel.HasTakenOnNewApprenticeships.HasValue)
            {
                ModelState.AddModelError("HasTakenOnNewApprenticeships", QualificationQuestionViewModel.HasTakenOnNewApprenticeshipsNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.HasTakenOnNewApprenticeships.Value)
            {
                return RedirectToAction("SelectApprenticeships", new { accountId });
            }

            return RedirectToAction("CannotApply", new { accountId });
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply()
        {
            return View(new CannotApplyViewModel(_configuration.CommitmentsBaseUrl));
        }

        [HttpGet]
        [Route("select-new-apprenticeships")]
        public async Task<ViewResult> SelectApprenticeships(string accountId)
        {
            var model = new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                // Apprenticeships = _apprenticesService.GetEligibleApprenticeships().OrderBy(a => a.LastName)
            };

            return View(model);
        }

        [HttpPost]
        [Route("select-new-apprenticeships")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, SelectApprenticeshipsViewModel viewModel)
        {
            //  viewModel.Apprenticeships = _service.GetEligibleApprenticeships().OrderBy(a => a.LastName);

            if (viewModel.HasSelectedApprenticeships)
            {
                return RedirectToAction("Declaration", new { accountId });
            }

            ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
            return View(viewModel);
        }

        [HttpGet]
        [Route("declaration")]
        public async Task<ViewResult> Declaration(string accountId)
        {
            return View(new DeclarationViewModel(accountId));
        }
    }
}