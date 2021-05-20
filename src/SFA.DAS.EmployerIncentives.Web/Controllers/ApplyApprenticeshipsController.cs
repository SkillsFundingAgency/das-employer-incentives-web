using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.Models;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{accountId}/apply")]
    public class ApplyApprenticeshipsController : ControllerBase
    {
        private readonly IApprenticesService _apprenticesService;
        private readonly IApplicationService _applicationService;
        private readonly ExternalLinksConfiguration _linksConfiguration;
        private readonly WebConfigurationOptions _webConfiguration;

        public ApplyApprenticeshipsController(
            IApprenticesService apprenticesService,
            IApplicationService applicationService,
            ILegalEntitiesService legalEntityService,
            IOptions<ExternalLinksConfiguration> linksConfiguration,
            IOptions<WebConfigurationOptions> webConfiguration) : base(legalEntityService)
        {
            _apprenticesService = apprenticesService;
            _applicationService = applicationService;
            _linksConfiguration = linksConfiguration.Value;
            _webConfiguration = webConfiguration.Value;
        } 

        [HttpGet]
        [Route("{accountLegalEntityId}/select-apprentices")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, string accountLegalEntityId, int offset = 0, int startIndex = 1)
        {
            var model = await GetInitialSelectApprenticeshipsViewModel(accountId, accountLegalEntityId, _webConfiguration.ApprenticeshipsPageSize, offset, startIndex);

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
            if (form.ApplicationId == Guid.Empty)
            {
                form.ApplicationId = await _applicationService.Create(form.AccountId, form.AccountLegalEntityId, form.SelectedApprenticeships);
            }
            else
            {
                await ProcessSelectedApprenticeships(form);
            }
            
            var model = await GetSelectApprenticeshipsViewModel(form.AccountId, form.ApplicationId, _webConfiguration.ApprenticeshipsPageSize, form.Offset, form.RequestedStartIndex);

            return View(model);
        }

        [HttpGet]
        [Route("select-apprentices/{applicationId}")]
        public async Task<IActionResult> SelectApprenticeships(string accountId, Guid applicationId, int offset = 0, int startIndex = 1)
        {
            var model = await GetSelectApprenticeshipsViewModel(accountId, applicationId, _webConfiguration.ApprenticeshipsPageSize, offset, startIndex);

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
            await ProcessSelectedApprenticeships(form);

            var model = await GetSelectApprenticeshipsViewModel(form.AccountId, applicationId, _webConfiguration.ApprenticeshipsPageSize, form.Offset, form.RequestedStartIndex);

            return View(model);
        }

        [HttpGet]
        [Route("confirm-apprentices/{applicationId}")]
        public async Task<IActionResult> ConfirmApprenticeships(string accountId, Guid applicationId)
        {
            var viewModel = await GetConfirmApprenticeshipViewModel(accountId, applicationId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("confirm-apprentices/{applicationId}")]
        public async Task<IActionResult> DisplayDeclaration(string accountId, Guid applicationId, bool newAgreementRequired)
        {
            if(newAgreementRequired)
                return RedirectToAction("NewAgreementRequired", new { accountId, applicationId });

            return RedirectToAction("Declaration", "Apply", new { accountId, applicationId });
        }

        [HttpPost]
        [Route("complete-apprentices/{applicationId}")]
        [Route("{accountLegalEntityId}/complete-apprentices")]
        public async Task<IActionResult> CompleteApprenticesSelection(SelectApprenticeshipsRequest form)
        {
            Guid applicationId = form.ApplicationId;

            if (applicationId == Guid.Empty)
            {
                applicationId = await _applicationService.Create(form.AccountId, form.AccountLegalEntityId, form.SelectedApprenticeships);
            }
            else
            {
                await ProcessSelectedApprenticeships(form);
            }

            var application = await _applicationService.Get(form.AccountId, applicationId, includeApprenticeships: true);

            if (!application.Apprentices.Any())
            {
                var viewModel = await GetSelectApprenticeshipsViewModel(form.AccountId, applicationId, _webConfiguration.ApprenticeshipsPageSize, form.Offset, form.StartIndex);
                ModelState.AddModelError(viewModel.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);

                return View("SelectApprenticeships", viewModel);
            }
            return RedirectToAction("ConfirmApprenticeships", new
            {
                form.AccountId,
                applicationId
            });
        }

        [HttpGet]
        [Route("accept-new-agreement/{applicationId}")]
        public async Task<IActionResult> NewAgreementRequired(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId, includeApprenticeships: false);
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            var viewModel = new NewAgreementRequiredViewModel(legalEntityName, accountId, applicationId, _linksConfiguration.ManageApprenticeshipSiteUrl);
            return View(viewModel);
        }

        private async Task ProcessSelectedApprenticeships(SelectApprenticeshipsRequest form)
        {
            if (form.SelectedApprenticeships == null)
            {
                form.SelectedApprenticeships = new List<string>();
            }
            var application = await _applicationService.Get(form.AccountId, form.ApplicationId, includeApprenticeships: true);
            var apprenticeshipIds = application.Apprentices.Select(x => x.ApprenticeshipId).ToList();

            var availableApprenticeships = await GetInitialSelectApprenticeshipsViewModel(form.AccountId,
                application.AccountLegalEntityId, _webConfiguration.ApprenticeshipsPageSize, form.Offset, form.StartIndex);
            var unselectedApprenticeships = availableApprenticeships.Apprenticeships
                .Where(x => !form.SelectedApprenticeships.Contains(x.Id)).Select(x => x.Id);
            var previousSelectedApprenticeships = application.Apprentices
                .Where(x => unselectedApprenticeships.Contains(x.ApprenticeshipId)).Select(x => x.ApprenticeshipId).ToList();
            if (form.SelectedApprenticeships.Any())
            {
                foreach(var apprenticeId in form.SelectedApprenticeships)
                {
                    if (application.Apprentices.FirstOrDefault(x => x.ApprenticeshipId == apprenticeId) == null)
                    {
                        apprenticeshipIds.Add(apprenticeId);
                    }
                }
            }
            if (previousSelectedApprenticeships.Any())
            {
                foreach(var apprenticeId in previousSelectedApprenticeships)
                {
                    apprenticeshipIds.Remove(apprenticeId);
                }
            }
            if (apprenticeshipIds.Any())
            {
                await _applicationService.Update(application.ApplicationId, application.AccountId, apprenticeshipIds);
            }
        }
        
        private async Task<SelectApprenticeshipsViewModel> GetInitialSelectApprenticeshipsViewModel(string accountId, string accountLegalEntityId, int pageSize, int offset, int startIndex)
        {
            var apprenticeships = await _apprenticesService.Get(new ApprenticesQuery(accountId, accountLegalEntityId, pageSize, offset, startIndex));
            var legalEntityName = await GetLegalEntityName(accountId, accountLegalEntityId);
            
            return new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                Apprenticeships = apprenticeships.Apprenticeships.OrderBy(a => a.DisplayName),
                OrganisationName = legalEntityName,
                PageSize = pageSize,
                MorePages = apprenticeships.MorePages,
                Offset = apprenticeships.Offset,
                StartIndex = startIndex
            };
        }

        private async Task<SelectApprenticeshipsViewModel> GetSelectApprenticeshipsViewModel(string accountId, Guid applicationId, int pageSize, int offset, int startIndex, bool showSelected = true)
        {
            var application = await _applicationService.Get(accountId, applicationId);
            var response = await _apprenticesService.Get(new ApprenticesQuery(accountId, application.AccountLegalEntityId, pageSize, offset, startIndex));
            var apprenticeships = response.Apprenticeships.ToList();

            if (showSelected)
            {
                apprenticeships.ForEach(x => x.Selected = application.Apprentices.Any(a => a.ApprenticeshipId == x.Id));
            }
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);
            return new SelectApprenticeshipsViewModel
            {
                AccountId = accountId,
                AccountLegalEntityId = application.AccountLegalEntityId,
                ApplicationId = applicationId,
                Apprenticeships = apprenticeships.OrderBy(a => a.DisplayName),
                OrganisationName = legalEntityName,
                PageSize = pageSize,
                MorePages = response.MorePages,
                Offset = response.Offset,
                StartIndex = response.StartIndex
            };
        }

        private async Task<ApplicationConfirmationViewModel> GetConfirmApprenticeshipViewModel(string accountId, Guid applicationId)
        {
            var application = await _applicationService.Get(accountId, applicationId);
            var legalEntityName = await GetLegalEntityName(accountId, application.AccountLegalEntityId);

            var apprenticeships = application.Apprentices.Select(MapFromApplicationApprenticeDto);
            return new ApplicationConfirmationViewModel(applicationId, accountId, application.AccountLegalEntityId,
                                                        apprenticeships, application.BankDetailsRequired, application.NewAgreementRequired, legalEntityName);
        }

        private ApplicationConfirmationViewModel.ApplicationApprenticeship MapFromApplicationApprenticeDto(ApplicationApprenticeshipModel apprentice)
        {
            return new ApplicationConfirmationViewModel.ApplicationApprenticeship
            {
                ApprenticeshipId = apprentice.ApprenticeshipId,
                CourseName = apprentice.CourseName,
                FirstName = apprentice.FirstName,
                LastName = apprentice.LastName,
                ExpectedAmount = apprentice.ExpectedAmount,
                StartDate = apprentice.StartDate,
                Uln = apprentice.Uln
            };
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously