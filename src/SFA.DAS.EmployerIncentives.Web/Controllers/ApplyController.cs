using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/[Controller]")]
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
                return RedirectToAction("SelectApprenticeships", new { accountId });
            }

            return RedirectToAction("CannotApply", new { accountId });
        }

        public async Task<ViewResult> SelectApprenticeships()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("select-new-apprentices")]
        public async Task<ViewResult> SelectApprentices()
        {
            return View(new SelectApprenticesViewModel());
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply()
        {
            return View(new CannotApplyViewModel(_configuration.CommitmentsBaseUrl));
        }
    }
}