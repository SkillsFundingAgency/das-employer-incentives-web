using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Cancel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/cancel")]
    public class CancelController : Controller
    {
        private readonly IApprenticeshipIncentiveService _apprenticeshipIncentiveService;
        private readonly ILegalEntitiesService _legalEntitiesService;

        public CancelController(
            IApprenticeshipIncentiveService apprenticeshipIncentiveService,
            ILegalEntitiesService legalEntitiesService)
        {
            _apprenticeshipIncentiveService = apprenticeshipIncentiveService;
            _legalEntitiesService = legalEntitiesService;
        } 

        [HttpGet]
        [Route("{accountLegalEntityId}/cancel-application")]
        public async Task<IActionResult> CancelApplication(string accountId, string accountLegalEntityId)
        {            
            var model = await GetSelectViewModel(accountId, accountLegalEntityId);

            if (!model.ApprenticeshipIncentives.Any())
            {
                return RedirectToAction("ListPaymentsForLegalEntity", "Payments", new { accountId, accountLegalEntityId });
            }            
            
            return View("SelectApprenticeships", model);
        }

        [HttpPost]
        [Route("{accountLegalEntityId}/confirm-cancel-application")]
        public async Task<IActionResult> Confirm(SelectApprenticeshipsRequest request)
        {
            if (!request.HasSelectedApprenticeships)
            {
                var viewModel = await GetSelectViewModel(request.AccountId, request.AccountLegalEntityId);

                ModelState.AddModelError(viewModel.FirstCheckboxId, viewModel.SelectApprenticeshipsMessage);

                return View("SelectApprenticeships", viewModel);
            }

            return View("ConfirmApprentices", await GetConfirmViewModel(request));
        }

        [HttpPost]
        [Route("{accountLegalEntityId}/application-cancelled")]
        public async Task<IActionResult> Cancelled(SelectApprenticeshipsRequest request, string accountId)
        {
            if (!request.HasSelectedApprenticeships)
            {
                return RedirectToAction("ListPaymentsForLegalEntity", "Payments", new { request.AccountId, request.AccountLegalEntityId });
            }

            var apprenticeshipIncentivesToWithdraw = 
                (await GetSelectViewModel(request.AccountId, request.AccountLegalEntityId))
                .ApprenticeshipIncentives.Where(a => a.Selected).ToList();

            var emailAddress = ControllerContext.HttpContext.User.FindFirst(c => c.Type.Equals(EmployerClaimTypes.EmailAddress))?.Value;

            await _apprenticeshipIncentiveService.Cancel(request.AccountLegalEntityId, apprenticeshipIncentivesToWithdraw,
                accountId, emailAddress);

            return View("ConfirmCancellation", await GetCancelledViewModel(request,  apprenticeshipIncentivesToWithdraw));
        }

        private async Task<SelectApprenticeshipsViewModel> GetSelectViewModel(string accountId, string accountLegalEntityId)
        {
            var apprenticeshipIncentives = await _apprenticeshipIncentiveService.GetList(accountId, accountLegalEntityId);
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);
            var model = new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                ApprenticeshipIncentives = apprenticeshipIncentives
                                        .OrderBy(a => a.FirstName)
                                        .ThenBy(a => a.LastName)
                                        .ThenBy(a => a.Uln),
                OrganisationName = legalEntity?.Name ?? string.Empty
            };

            if (TempData["selected"] is string[] selected)
            {
                foreach (var incentive in model.ApprenticeshipIncentives)
                {
                    if (selected.Contains(incentive.Id))
                    {
                        incentive.Selected = true;
                    }
                }
                TempData.Clear();
            }

            return model;
        }

        private async Task<ConfirmApprenticeshipsViewModel> GetConfirmViewModel(SelectApprenticeshipsRequest request)
        {
            var viewModel = await GetSelectViewModel(request.AccountId, request.AccountLegalEntityId);

            TempData.Add("selected", request.SelectedApprenticeships);

            return new ConfirmApprenticeshipsViewModel
            {
                AccountId = viewModel.AccountId,
                AccountLegalEntityId = viewModel.AccountLegalEntityId,
                ApprenticeshipIncentives = viewModel
                                        .ApprenticeshipIncentives
                                        .Where(a => request.SelectedApprenticeships.Contains(a.Id))
                                        .OrderBy(a => a.FirstName)
                                        .ThenBy(a => a.LastName)
                                        .ThenBy(a => a.Uln),
                OrganisationName = viewModel.OrganisationName
            };
        }

        private async Task<CancelledApprenticeshipsViewModel> GetCancelledViewModel(
            SelectApprenticeshipsRequest request, 
            IEnumerable<ApprenticeshipIncentiveModel> apprenticeshipIncentives)
        {
            var legalEntity = await _legalEntitiesService.Get(request.AccountId, request.AccountLegalEntityId);

            return new CancelledApprenticeshipsViewModel
            {
                AccountId = request.AccountId,
                AccountLegalEntityId = request.AccountLegalEntityId,
                ApprenticeshipIncentives = apprenticeshipIncentives.OrderBy(a => a.Uln),
                OrganisationName = legalEntity?.Name ?? string.Empty
            };
        }
    }
}