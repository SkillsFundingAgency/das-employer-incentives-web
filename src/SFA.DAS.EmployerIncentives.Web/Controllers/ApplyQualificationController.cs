using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyQualificationController : ControllerBase
    {
        public ApplyQualificationController(ILegalEntitiesService legalEntitiesService) : base(legalEntitiesService)
        {
        }

        [Route("{accountLegalEntityId}/eligible-apprentices")]
        [HttpGet]
        public async Task<IActionResult> GetQualificationQuestion(string accountId, string accountLegalEntityId)
        {
            var viewModel = new QualificationQuestionViewModel 
            {
                OrganisationName = await GetLegalEntityName(accountId, accountLegalEntityId)
            };
            return View("QualificationQuestion", viewModel);
        }

        [Route("{accountLegalEntityId}/eligible-apprentices")]
        [HttpPost]
        public IActionResult QualificationQuestion(QualificationQuestionViewModel viewModel)
        {
            if (!viewModel.HasTakenOnNewApprentices.HasValue)
            {
                ModelState.AddModelError("HasTakenOnNewApprentices", QualificationQuestionViewModel.HasTakenOnNewApprenticesNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.HasTakenOnNewApprentices.Value)
            {
                return RedirectToAction("ValidateTermsSigned", "ApplyOrganisation", new { viewModel.AccountId, viewModel.AccountLegalEntityId });
            }

            return RedirectToAction("CannotApply", "Apply", new { accountId = viewModel.AccountId, accountLegalEntityId = viewModel.AccountLegalEntityId });            
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/taken-on-new-apprentices")]
        public IActionResult Redirect()
        {
            return RedirectToActionPermanent("GetQualificationQuestion");
        }
    }
}