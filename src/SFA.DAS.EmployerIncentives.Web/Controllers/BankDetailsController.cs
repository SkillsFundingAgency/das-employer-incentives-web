using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{applicationId}/bankdetails")]
    public class BankDetailsController : Controller
    {
        [HttpGet]
        [Route("need-bank-details")]
        public ViewResult BankDetailsConfirmation(Guid applicationId)
        {
            return View(new BankDetailsConfirmationViewModel { ApplicationId = applicationId });
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
        public ViewResult AddBankDetails(Guid applicationId)
        {
            return View();
        }

        [HttpPost]
        [Route("enter-bank-details")]
        public ViewResult EnterBankDetails(Guid applicationId)
        {
            // Once integration mechanism is finalised, redirect / post to external site
            return View();
        }

        [HttpGet]
        [Route("complete/need-bank-details")]
        public ViewResult NeedBankDetails(Guid applicationId)
        {
            return View();
        }
    }
}
