﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Extensions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/payments")]
    public class PaymentsController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly ExternalLinksConfiguration _linksConfiguration;
        private readonly WebConfigurationOptions _webConfiguration;

        public PaymentsController(IApplicationService applicationService, ILegalEntitiesService legalEntitiesService, 
                                  IOptions<ExternalLinksConfiguration> linksConfiguration,
                                  IOptions<WebConfigurationOptions> webConfiguration)
        {
            _applicationService = applicationService;
            _legalEntitiesService = legalEntitiesService;
            _linksConfiguration = linksConfiguration.Value;
            _webConfiguration = webConfiguration.Value;
        }

        [Route("payment-applications")]
        public async Task<IActionResult> ListPayments(string accountId, string sortOrder, string sortField)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);

            if (legalEntities.Count() > 1) 
            {                
                return RedirectToAction("ChooseOrganisation", new { accountId });
            }

            return RedirectToAction("ListPaymentsForLegalEntity", new { accountId, legalEntities.First().AccountLegalEntityId, sortOrder, sortField });
        }       

        [Route("{accountLegalEntityId}/payment-applications")]
        public async Task<IActionResult> ListPaymentsForLegalEntity(string accountId, string accountLegalEntityId, string sortOrder, string sortField)
        {
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                sortOrder = ApplicationsSortOrder.Ascending;
            }
            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = ApplicationsSortField.ApprenticeName;
            }

            var getApplicationsResponse = await _applicationService.GetList(accountId, accountLegalEntityId);

            var submittedApplications = getApplicationsResponse.ApprenticeApplications.AsQueryable();

            if (!submittedApplications.Any())
            {
                return RedirectToAction("NoApplications", new { accountId, accountLegalEntityId });
            }
            var viewAgreementLink = CreateViewAgreementLink(accountId);
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);

            //EI-896 - emergency fudge to stop the Paused/Withdrawn message being displayed for anyone with a payment.
            foreach (var apprenticeApplicationModel in submittedApplications)
            {
                if (apprenticeApplicationModel.FirstPaymentStatus != null)
                {
                    apprenticeApplicationModel.FirstPaymentStatus.ViewAgreementLink = viewAgreementLink;
                    apprenticeApplicationModel.FirstPaymentStatus.InLearning = true;
                    apprenticeApplicationModel.FirstPaymentStatus.IsClawedBack = apprenticeApplicationModel.FirstClawbackStatus != null && apprenticeApplicationModel.FirstClawbackStatus.IsClawedBack;
                }

                if (apprenticeApplicationModel.SecondPaymentStatus != null)
                {
                    apprenticeApplicationModel.SecondPaymentStatus.ViewAgreementLink = viewAgreementLink;
                    apprenticeApplicationModel.SecondPaymentStatus.InLearning = true;
                    apprenticeApplicationModel.SecondPaymentStatus.IsClawedBack = apprenticeApplicationModel.SecondClawbackStatus != null && apprenticeApplicationModel.SecondClawbackStatus.IsClawedBack;
                }
            }

            submittedApplications = SortApplications(sortOrder, sortField, submittedApplications, _webConfiguration.DisplayEmploymentCheckResult);

            var model = new ViewApplicationsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                Applications = submittedApplications,
                SortField = sortField,
                ShowBankDetailsInReview = getApplicationsResponse.BankDetailsStatus == BankDetailsStatus.InProgress,
                ShowAddBankDetailsCalltoAction = getApplicationsResponse.BankDetailsStatus == BankDetailsStatus.NotSupplied || getApplicationsResponse.BankDetailsStatus == BankDetailsStatus.Rejected,
                AddBankDetailsLink = CreateAddBankDetailsLink(accountId, getApplicationsResponse.FirstSubmittedApplicationId),
                ShowAcceptNewEmployerAgreement = getApplicationsResponse.ApprenticeApplications.Any(a => (a.FirstPaymentStatus != null && a.FirstPaymentStatus.RequiresNewEmployerAgreement) || (a.SecondPaymentStatus != null && a.SecondPaymentStatus.RequiresNewEmployerAgreement)),
                OrganisationName = legalEntity?.Name,
                ViewAgreementLink = viewAgreementLink,
                ShowCancelLink = getApplicationsResponse.ApprenticeApplications.Any(a => !a.IsWithdrawn),
                CancelLink = CreateCancelApprenticesLink(accountId, accountLegalEntityId)
            };
            model.SetSortOrder(sortField, sortOrder);

            return View("ListPayments", model);
        }

        [Route("{accountLegalEntityId}/no-applications")]
        public async Task<IActionResult> NoApplications(string accountId, string accountLegalEntityId)
        {
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);
            var model = new NoApplicationsViewModel
            { 
                OrganisationName = legalEntity?.Name, 
                AccountId = accountId, 
                AccountLegalEntityId = accountLegalEntityId 
            };
            return View(model);
        }

        [Route("choose-organisation")]
        [HttpGet]
        public async Task<IActionResult> ChooseOrganisation(string accountId)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);
            var model = new ChooseOrganisationViewModel(_linksConfiguration.ManageApprenticeshipSiteUrl, accountId) 
            { 
                LegalEntities = legalEntities 
            };
            return View(model);
        }

        [Route("choose-organisation")]
        [HttpPost]
        public async Task<IActionResult> ChooseOrganisation(ChooseOrganisationViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Selected))
            {
                return RedirectToAction("ListPaymentsForLegalEntity", new { model.AccountId, accountLegalEntityId = model.Selected });
            }

            model.LegalEntities = await _legalEntitiesService.Get(model.AccountId);

            if (string.IsNullOrEmpty(model.Selected))
            {
                ModelState.AddModelError(model.LegalEntities.Any() ? model.LegalEntities.First().AccountLegalEntityId : "LegalEntityNotSelected", model.LegalEntityNotSelectedMessage);
            }

            return View(model);
        }

        private static IQueryable<ApprenticeApplicationModel> SortApplications(string sortOrder, string sortField, IQueryable<ApprenticeApplicationModel> submittedApplications, bool employmentCheckFeatureToggle)
        {
            if (sortOrder == ApplicationsSortOrder.Descending)
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    submittedApplications = submittedApplications.OrderByDescending(sortField).ThenBy(x => x.ULN).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    submittedApplications = submittedApplications.OrderByDescending(sortField).ThenBy(x => x.ULN);
                }
            }
            else
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    submittedApplications = submittedApplications.OrderBy(sortField).ThenBy(x => x.ULN).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    submittedApplications = submittedApplications.OrderBy(sortField).ThenBy(x => x.ULN);
                }
            }

            return submittedApplications.Select(x => ApplyEmploymentCheckFeatureToggle(x, employmentCheckFeatureToggle));
        }

        private static ApprenticeApplicationModel ApplyEmploymentCheckFeatureToggle(ApprenticeApplicationModel application, bool employmentCheckFeatureToggle)
        {
            if (application.FirstPaymentStatus != null)
            {
                application.FirstPaymentStatus.DisplayEmploymentCheckResult = employmentCheckFeatureToggle;
            }
            if (application.SecondPaymentStatus != null)
            {
                application.SecondPaymentStatus.DisplayEmploymentCheckResult = employmentCheckFeatureToggle;
            }

            return application;
        }

        private string CreateAddBankDetailsLink(string accountId, Guid? firstSubmittedApplicationId)
        {
            var requestContext = ControllerContext.HttpContext.Request;
            var host = $"{requestContext.Scheme}://{requestContext.Host}";
            var bankDetailsUrl = $"{host}/{accountId}/bank-details/{firstSubmittedApplicationId}/add-bank-details";
            return bankDetailsUrl;
        }

        private string CreateCancelApprenticesLink(string accountId, string accountLegalEntityId)
        {
            var requestContext = ControllerContext.HttpContext.Request;
            var host = $"{requestContext.Scheme}://{requestContext.Host}";
            return $"{host}/{accountId}/cancel/{accountLegalEntityId}/cancel-application";
        }

        private string CreateViewAgreementLink(string accountId)
        {
            var accountsbaseUrl = _linksConfiguration.ManageApprenticeshipSiteUrl;
            if (!accountsbaseUrl.EndsWith("/"))
            {
                accountsbaseUrl += "/";
            }
            return $"{accountsbaseUrl}accounts/{accountId}/agreements";
        }
    }
}
