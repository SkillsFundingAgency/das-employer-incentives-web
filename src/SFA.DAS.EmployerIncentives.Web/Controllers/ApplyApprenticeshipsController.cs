﻿using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyApprenticeshipsController : ControllerBase
    {
        private readonly IApprenticesService _apprenticesService;
        private readonly IApplicationService _applicationService;

        public ApplyApprenticeshipsController(
            IApprenticesService apprenticesService,
            IApplicationService applicationService,
            ILegalEntitiesService legalEntityService) : base(legalEntityService)
        {
            _apprenticesService = apprenticesService;
            _applicationService = applicationService;
        } 

        [HttpGet]
        [Route("{accountLegalEntityId}/select-apprentices")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, string accountLegalEntityId)
        {
            var model = await GetInitialSelectApprenticeshipsViewModel(accountId, accountLegalEntityId);

            if (!model.Apprenticeships.Any())
            {
                return RedirectToAction("CannotApplyYet", "Apply", new { accountId, accountLegalEntityId });
            }

            return View(model);
        }

        [HttpPost]
        [Route("{accountLegalEntityId}/select-apprentices")]
        public async Task<IActionResult> SelectApprenticeships(SelectApprenticeshipsRequest form)
        {
            if (form.HasSelectedApprenticeships)
            {
                var applicationId = await _applicationService.Create(form.AccountId, form.AccountLegalEntityId, form.SelectedApprenticeships);
                return RedirectToAction("EmploymentStartDates", "ApplyEmploymentDetails", new { form.AccountId, applicationId });
            }

            var viewModel = await GetInitialSelectApprenticeshipsViewModel(form.AccountId, form.AccountLegalEntityId);
            ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);

            return View(viewModel);
        }

        [HttpGet]
        [Route("select-apprentices/{applicationId}")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, Guid applicationId)
        {
            var model = await GetSelectApprenticeshipsViewModel(accountId, applicationId);
            if (model == null)
            {
                return RedirectToAction("Home", "Home", new { accountId });
            }
            if (!model.Apprenticeships.Any())
            {
                return RedirectToAction("CannotApplyYet", "Apply", new { accountId, accountLegalEntityId = model.AccountLegalEntityId });
            }

            return View(model);
        }

        [HttpPost]
        [Route("select-apprentices/{applicationId}")]
        public async Task<IActionResult> SelectApprenticeships(SelectApprenticeshipsRequest form, Guid applicationId)
        {
            if (form.HasSelectedApprenticeships)
            {
                await _applicationService.Update(applicationId, form.AccountId, form.SelectedApprenticeships);
                return RedirectToAction("EmploymentStartDates", "ApplyEmploymentDetails", new { form.AccountId, applicationId });
            }

            var viewModel = await GetSelectApprenticeshipsViewModel(form.AccountId, applicationId, false);
            if (viewModel == null)
            {
                return RedirectToAction("Home", "Home", new { form.AccountId });
            }    
            ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);

            return View(viewModel);
        }

        [HttpGet]
        [Route("confirm-apprentices/{applicationId}")]
        public async Task<IActionResult> ConfirmApprenticeships(string accountId, Guid applicationId, bool all = true)
        {
            var viewModel = await GetConfirmApprenticeshipViewModel(accountId, applicationId);
            if(viewModel == null)
            {
                return RedirectToAction("Home", "Home", new { accountId });
            }

            if (all && viewModel.HasIneligibleApprentices)
            {
                return View("NotEligibleApprenticeships", new NotEligibleViewModel(viewModel));
            }

            viewModel.Apprentices.RemoveAll(apprentice => !apprentice.StartDatesAreEligible);
            
            return View(viewModel);
        }

        [HttpPost]
        [Route("confirm-apprentices/{applicationId}")]
        public IActionResult DisplayDeclaration(string accountId, Guid applicationId)
        {
            return RedirectToAction("Declaration", "Apply", new { accountId, applicationId });
        }

        private async Task<SelectApprenticeshipsViewModel> GetInitialSelectApprenticeshipsViewModel(string accountId, string accountLegalEntityId)
        {
            var apprenticeships = await _apprenticesService.Get(new ApprenticesQuery(accountId, accountLegalEntityId));
            var legalEntityName = await GetLegalEntityName(accountId, accountLegalEntityId);
            return new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                Apprenticeships = apprenticeships.OrderBy(a => a.LastName),
                OrganisationName = legalEntityName
            };
        }

        private async Task<SelectApprenticeshipsViewModel> GetSelectApprenticeshipsViewModel(string accountId, Guid applicationId, bool showSelected = true)
        {
            var application = await _applicationService.Get(accountId, applicationId);
            if (application == null)
            {
                return null;
            }

            var apprenticeships = (await _apprenticesService.Get(new ApprenticesQuery(accountId, application.AccountLegalEntityId))).ToList();

            if (showSelected)
            {
                apprenticeships.ForEach(x => x.Selected = application.Apprentices.Any(a => a.ApprenticeshipId == x.Id));
            }
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            return new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = application.AccountLegalEntityId,
                Apprenticeships = apprenticeships.OrderBy(a => a.LastName),
                OrganisationName = legalEntityName
            };
        }


        private async Task<ApplicationConfirmationViewModel> GetConfirmApprenticeshipViewModel(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId);
            if(application == null)
            {
                return null;
            }
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);

            var apprenticeships = application.Apprentices.Select(MapFromApplicationApprenticeDto);
            return new ApplicationConfirmationViewModel(applicationId, accountId, application.AccountLegalEntityId,
                                                        apprenticeships, application.BankDetailsRequired, legalEntityName);
        }

        private ApplicationApprenticeship MapFromApplicationApprenticeDto(ApplicationApprenticeshipModel apprentice)
        {
            return new ApplicationApprenticeship
            {
                ApprenticeshipId = apprentice.ApprenticeshipId,
                CourseName = apprentice.CourseName,
                FirstName = apprentice.FirstName,
                LastName = apprentice.LastName,
                ExpectedAmount = apprentice.ExpectedAmount,
                StartDate = apprentice.StartDate,
                Uln = apprentice.Uln,
                EmploymentStartDate = apprentice.EmploymentStartDate,
                StartDatesAreEligible = apprentice.StartDatesAreEligible
            };
        }
    }
}