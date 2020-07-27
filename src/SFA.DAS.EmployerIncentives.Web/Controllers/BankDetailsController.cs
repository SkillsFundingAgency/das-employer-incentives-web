using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class BankDetailsController : Controller
    {
        [HttpGet]
        [Route("need-bank-details")]
        public ViewResult BankDetailsConfirmation(string accountId, string accountLegalEntityId)
        {
            return View(new BankDetailsConfirmationViewModel { AccountId = accountId, AccountLegalEntityId = accountLegalEntityId });
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
                // redirect to business central
                return RedirectToAction("EnterBankDetails");
            }

            // redirect to need bank details page
            return RedirectToAction("NeedBankDetails");
        }


        [HttpGet]
        [Route("enter-bank-details")]
        public ViewResult EnterBankDetails()
        {
            // Once integration mechanism is finalised, redirect / post to external site
            return View();
        }

        [HttpGet]
        [Route("complete/need-bank-details")]
        public ViewResult NeedBankDetails()
        {
            return View();
        }
    }
}
