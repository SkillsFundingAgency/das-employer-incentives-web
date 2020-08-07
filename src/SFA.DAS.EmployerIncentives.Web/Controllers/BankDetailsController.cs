using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/bankdetails")]
    public class BankDetailsController : Controller
    {
        [HttpGet]
        [Route("{accountLegalEntityId}/need-bank-details")]
        public ViewResult BankDetailsConfirmation(string accountId, string accountLegalEntityId)
        {
            return View(new BankDetailsConfirmationViewModel { AccountId = accountId, AccountLegalEntityId = accountLegalEntityId });
        }

        [HttpPost]
        [Route("{accountLegalEntityId}/need-bank-details")]
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
        [Route("{accountLegalEntityId}/add-bank-details")]
        public ViewResult AddBankDetails()
        {
            return View();
        }

        [HttpPost]
        [Route("{accountLegalEntityId}/enter-bank-details")]
        public ViewResult EnterBankDetails()
        {
            // Once integration mechanism is finalised, redirect / post to external site
            return View();
        }

        [HttpGet]
        [Route("{accountLegalEntityId}/complete/need-bank-details")]
        public ViewResult NeedBankDetails()
        {
            return View();
        }
    }
}
