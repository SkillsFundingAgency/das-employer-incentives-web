using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.Email.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/bank-details/{applicationId}")]
    public class BankDetailsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IApplicationService _applicationService;
        private readonly IHashingService _hashingService;
        private readonly IVerificationService _verificationService;
        private readonly ILegalEntitiesService _legalEntitiesService;
        private ExternalLinksConfiguration _configuration;

        public BankDetailsController(IVerificationService verificationService,
            IEmailService emailService,
            IApplicationService applicationService,
            IHashingService hashingService,
            ILegalEntitiesService legalEntitiesService,
            IOptions<ExternalLinksConfiguration> configuration) : base(legalEntitiesService)
        {
            _verificationService = verificationService;
            _emailService = emailService;
            _applicationService = applicationService;
            _hashingService = hashingService;
            _legalEntitiesService = legalEntitiesService;
            _configuration = configuration.Value;
        }

        [HttpGet]
        [Route("need-bank-details")]
        public async Task<IActionResult> BankDetailsConfirmation(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);

            if (!application.BankDetailsRequired)
            {
                return RedirectToAction("Confirmation", "ApplicationComplete");
            }
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            return View(new BankDetailsConfirmationViewModel { AccountId = accountId, ApplicationId = applicationId, OrganisationName = legalEntityName });
        }

        [HttpPost]
        [Route("need-bank-details")]
        public async Task<IActionResult> BankDetailsConfirmation(BankDetailsConfirmationViewModel viewModel)
        {
            if (!viewModel.CanProvideBankDetails.HasValue)
            {
                ModelState.AddModelError("CanProvideBankDetails", viewModel.CanProvideBankDetailsNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.CanProvideBankDetails.Value)
            {
                // redirect to interstitial page
                await SendBankDetailsReminderEmail(viewModel.AccountId, viewModel.ApplicationId);
                return RedirectToAction("AddBankDetails", new { accountId = viewModel.AccountId, applicationId = viewModel.ApplicationId });
            }

            // redirect to need bank details page
            await SendBankDetailsRequiredEmail(viewModel.AccountId, viewModel.ApplicationId);
            return RedirectToAction("NeedBankDetails");
        }

        [HttpGet]
        [Route("add-bank-details")]
        public async Task<IActionResult> AddBankDetails(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            var model = new AddBankDetailsViewModel { OrganisationName = legalEntityName };
            return View(model);
        }

        [HttpPost]
        [Route("enter-bank-details")]
        public async Task<IActionResult> EnterBankDetails(string accountId, Guid applicationId)
        {
            var confirmationActionUrl = Url.Action("Confirmation", "ApplicationComplete", new { accountId, applicationId });
            var returnUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{confirmationActionUrl}";
            var application = await _applicationService.Get(accountId, applicationId, false);

            var achieveServiceUrl = await _verificationService.BuildAchieveServiceUrl(accountId, application.AccountLegalEntityId, applicationId, returnUrl);

            return Redirect(achieveServiceUrl);
        }

        [HttpGet]
        [Route("complete/need-bank-details")]
        public ViewResult NeedBankDetails(Guid applicationId)
        {
            var model = new NeedBankDetailsViewModel { AccountHomeUrl = _configuration.ManageApprenticeshipSiteUrl };
            return View(model);
        }

        [HttpGet]
        [Route("change-bank-details")]
        public async Task<IActionResult> AmendBankDetails(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, false);
            var legalEntity = await _legalEntitiesService.Get(accountId, application.AccountLegalEntityId);
            var model = new AmendBankDetailsViewModel(accountId, application.AccountLegalEntityId, applicationId, legalEntity.Name);
            return View(model);
        }

        [HttpPost]
        [Route("change-bank-details")]
        public async Task<IActionResult> AmendBankDetails(AmendBankDetailsViewModel model)
        {
            var confirmationActionUrl = Url.Action("Index", "Hub", new { model.AccountId, model.AccountLegalEntityId });
            var returnUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{confirmationActionUrl}";
            var achieveServiceUrl = await _verificationService.BuildAchieveServiceUrl(model.AccountId, model.AccountLegalEntityId, model.ApplicationId, returnUrl, amendBankDetails: true);

            return Redirect(achieveServiceUrl);
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
            var bankDetailsUrl = $"{host}/{accountId}/bank-details/{applicationId}/add-bank-details";
            return bankDetailsUrl;
        }
    }
}
