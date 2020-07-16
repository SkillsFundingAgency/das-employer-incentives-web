using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyController : Controller
    {
        private readonly EmployerIncentivesWebConfiguration _configuration;

        public ApplyController(IOptions<EmployerIncentivesWebConfiguration> configuration)
        {
            _configuration = configuration.Value;
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
            if (!viewModel.HasTakenOnNewApprentices.HasValue)
            {
                ModelState.AddModelError("HasTakenOnNewApprentices", QualificationQuestionViewModel.HasTakenOnNewApprenticesNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.HasTakenOnNewApprentices.Value)
            {
                return RedirectToAction("SelectApprentices", new { accountId });
            }

            return RedirectToAction("CannotApply", new { accountId });
        }


        [HttpGet]
        [Route("select-new-apprentices")]
        public async Task<ViewResult> SelectApprentices(string accountId)
        {
            var model = new SelectApprenticesViewModel { AccountId = accountId };

            return View(model);
        }

        [HttpPost]
        [Route("select-new-apprentices")]
        public async Task<IActionResult> SelectApprentices(string accountId, SelectApprenticesViewModel viewModel)
        {
            if (viewModel.HasSelectedApprentices)
            {
                return RedirectToAction("Declaration", new { accountId });
            }
            else
            {
                ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticesViewModel.SelectApprenticesMessage);
                return View(viewModel);
            }
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply()
        {
            return View(new CannotApplyViewModel(_configuration.CommitmentsBaseUrl));
        }

        [HttpGet]
        [Route("declaration")]
        public async Task<ViewResult> Declaration(string accountId)
        {
            return View(new DeclarationViewModel(accountId));
        }
    }
}