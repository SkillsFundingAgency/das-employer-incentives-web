using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/bank-details/{applicationId}")]
    public class BankDetailsController : Controller
    {
        private readonly IVerificationService _verificationService;

        public BankDetailsController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpGet]
        [Route("need-bank-details")]
        public ViewResult BankDetailsConfirmation(string accountId, Guid applicationId)
        {
            return View(new BankDetailsConfirmationViewModel { AccountId = accountId, ApplicationId = applicationId });
        }

        [HttpPost]
        [Route("need-bank-details")]
        public IActionResult BankDetailsConfirmation(BankDetailsConfirmationViewModel viewModel)
        {
            if (!viewModel.CanProvideBankDetails.HasValue)
            {
                ModelState.AddModelError("CanProvideBankDetails", BankDetailsConfirmationViewModel.CanProvideBankDetailsNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.CanProvideBankDetails.Value)
            {
                // redirect to interstitial page
                return RedirectToAction("AddBankDetails");
            }

            // redirect to need bank details page
            return RedirectToAction("NeedBankDetails");
        }

        [HttpGet]
        [Route("add-bank-details")]
        public async Task<IActionResult> AddBankDetails(string accountId, Guid applicationId)
        {
            var returnUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{ApplicationCompleteController.ApplicationCompleteRoute}";
            var achieveServiceUrl = await _verificationService.BuildAchieveServiceUrl(accountId, applicationId, returnUrl);

            return Redirect(achieveServiceUrl);
        }

        [HttpGet]
        [Route("complete/need-bank-details")]
        public ViewResult NeedBankDetails(Guid applicationId)
        {
            return View();
        }
    }
}
