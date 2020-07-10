using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/[Controller]")]
    public class ApplyController : Controller
    {
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
                viewModel.AddError("HasTakenOnNewApprentices", "Select yes if you’ve taken on new apprentices that joined your payroll after 1 August 2020");
                return View(viewModel);
            }

            if (viewModel.HasTakenOnNewApprentices.Value)
            {
                return RedirectToAction("SelectApprenticeships");
            }

            return RedirectToAction("CannotApply");
        }

        public async Task<ViewResult> SelectApprenticeships()
        {
            throw new NotImplementedException();
        }

        public async Task<ViewResult> CannotApply()
        {
            throw new NotImplementedException();
        }
    }
}