using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using SFA.DAS.EmployerIncentives.Web.Extensions;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using System.Collections.Generic;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/payments")]
    public class PaymentsController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly IHashingService _hashingService;

        public PaymentsController(IApplicationService applicationService, ILegalEntitiesService legalEntitiesService, IHashingService hashingService)
        {
            _applicationService = applicationService;
            _legalEntitiesService = legalEntitiesService;
            _hashingService = hashingService;
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
            var submittedApplications = getApplicationsResponse.ApprenticeApplications.Where(x => x.Status == "Submitted").AsQueryable();

            if (!submittedApplications.Any())
            {
                return RedirectToAction("NoApplications", new { accountId, accountLegalEntityId });
            }

            submittedApplications = SortApplications(sortOrder, sortField, submittedApplications);

            var model = new ViewApplicationsViewModel
            {
                Applications = submittedApplications,
                SortField = sortField,
                BankDetailsStatus = getApplicationsResponse.BankDetailsStatus,
                AddBankDetailsLink = CreateAddBankDetailsLink(submittedApplications)
            };
            model.SetSortOrder(sortField, sortOrder);

            return View("ListPayments", model);
        }

        [Route("{accountLegalEntityId}/no-applications")]
        public ViewResult NoApplications()
        {
            var model = new NoApplicationsViewModel();
            return View(model);
        }

        [Route("choose-organisation")]
        [HttpGet]
        public async Task<IActionResult> ChooseOrganisation(string accountId)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);
            var model = new ChooseOrganisationViewModel { AccountId = accountId, LegalEntities = legalEntities };
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

        private static IQueryable<ApprenticeApplicationModel> SortApplications(string sortOrder, string sortField, IQueryable<ApprenticeApplicationModel> submittedApplications)
        {
            if (sortOrder == ApplicationsSortOrder.Descending)
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    submittedApplications = submittedApplications.OrderByDescending(sortField).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    submittedApplications = submittedApplications.OrderByDescending(sortField);
                }
            }
            else
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    submittedApplications = submittedApplications.OrderBy(sortField).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    submittedApplications = submittedApplications.OrderBy(sortField);
                }
            }

            return submittedApplications;
        }

        private string CreateAddBankDetailsLink(IEnumerable<ApprenticeApplicationModel> applications)
        {
            var application = applications.OrderBy(x => x.ApplicationDate).FirstOrDefault();
            var requestContext = ControllerContext.HttpContext.Request;
            var host = $"{requestContext.Scheme}://{requestContext.Host}";
            var hashedAccountId = _hashingService.HashValue(application.AccountId);
            var bankDetailsUrl = $"{host}/{hashedAccountId}/bank-details/{application.ApplicationId}/add-bank-details";
            return bankDetailsUrl;
        }
    }
}
