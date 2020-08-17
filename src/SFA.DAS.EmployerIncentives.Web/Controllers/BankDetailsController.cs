using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.Email.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/bankdetails/{applicationId}")]
    public class BankDetailsController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IApplicationService _applicationService;
        private readonly IHashingService _hashingService;

        public BankDetailsController(IEmailService emailService, IApplicationService applicationService, IHashingService hashingService)
        {
            _emailService = emailService;
            _applicationService = applicationService;
            _hashingService = hashingService;
        }

        [HttpGet]
        [Route("need-bank-details")]
        public ViewResult BankDetailsConfirmation(string accountId, Guid applicationId)
        {
            return View(new BankDetailsConfirmationViewModel { AccountId = accountId, ApplicationId = applicationId });
        }

        [HttpPost]
        [Route("need-bank-details")]
        public async Task<IActionResult> BankDetailsConfirmation(BankDetailsConfirmationViewModel viewModel)
        {
            if (!viewModel.CanProvideBankDetails.HasValue)
            {
                ModelState.AddModelError("CanProvideBankDetails", BankDetailsConfirmationViewModel.CanProvideBankDetailsNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.CanProvideBankDetails.Value)
            {
                // redirect to interstitial page
                await SendBankDetailsReminderEmail(viewModel.AccountId, viewModel.ApplicationId);
                return RedirectToAction("AddBankDetails");
            }

            // redirect to need bank details page
            await SendBankDetailsRequiredEmail(viewModel.AccountId, viewModel.ApplicationId);
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

        private async Task SendBankDetailsRequiredEmail(string accountId, Guid applicationId)
        {
            var bankDetailsUrl = CreateAddBankDetailsUrl(accountId, applicationId);

            var sendEmailRequest = await CreateSendBankDetailsEmailRequest(accountId, applicationId, bankDetailsUrl);

            await _emailService.SendBankDetailsRequiredEmail(sendEmailRequest);
        }

        private async Task SendBankDetailsReminderEmail(string accountId, Guid applicationId)
        {
            var bankDetailsUrl = CreateAddBankDetailsUrl(accountId, applicationId);

            var sendEmailRequest = await CreateSendBankDetailsEmailRequest(accountId, applicationId, bankDetailsUrl);

            await _emailService.SendBankDetailsReminderEmail(sendEmailRequest);
        }

        private async Task<SendBankDetailsEmailRequest> CreateSendBankDetailsEmailRequest(string accountId, Guid applicationId, string bankDetailsUrl)
        {
            var accountLegalEntityId = await _applicationService.GetApplicationLegalEntity(accountId, applicationId);

            var emailAddress = ControllerContext.HttpContext.User.FindFirst(c => c.Type.Equals(EmployerClaimTypes.EmailAddress))?.Value;

            var sendEmailRequest = new SendBankDetailsEmailRequest
            {
                AccountId = _hashingService.DecodeValue(accountId),
                AccountLegalEntityId = accountLegalEntityId,
                EmailAddress = emailAddress,
                AddBankDetailsUrl = bankDetailsUrl
            };
            return sendEmailRequest;
        }

        private string CreateAddBankDetailsUrl(string accountId, Guid applicationId)
        {
            var requestContext = ControllerContext.HttpContext.Request;
            var host = $"{requestContext.Scheme}://{requestContext.Host}";
            var bankDetailsUrl = $"{host}/{accountId}/bankdetails/{applicationId}/add-bank-details";
            return bankDetailsUrl;
        }
    }
}
