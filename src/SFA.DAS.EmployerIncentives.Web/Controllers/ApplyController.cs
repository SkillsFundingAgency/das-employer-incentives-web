using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Linq;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("{hashedAccountId}/[Controller]")]
    public class ApplyController : Controller
    {
        private readonly WebConfigurationOptions _configuration;
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly IApprenticesService _apprenticesService;
        private readonly IHashingService _hashingService;

        public ApplyController(
            IOptions<WebConfigurationOptions> configuration,
            ILegalEntitiesService legalEntitiesService,
            IApprenticesService apprenticesService,
            IHashingService hashingService)
        {
            _configuration = configuration.Value;
            _legalEntitiesService = legalEntitiesService;
            _apprenticesService = apprenticesService;
            _hashingService = hashingService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> QualificationQuestion()
        {
            return View(new QualificationQuestionViewModel());
        }

        [Route("")]
        [HttpPost]        
        public async Task<IActionResult> QualificationQuestion(string hashedAccountId, QualificationQuestionViewModel viewModel)
        {
            if (!viewModel.HasTakenOnNewApprentices.HasValue)
            {
                ModelState.AddModelError("HasTakenOnNewApprentices", QualificationQuestionViewModel.HasTakenOnNewApprenticesNotSelectedMessage);
                return View(viewModel);
            }

            if (viewModel.HasTakenOnNewApprentices.Value)
            {
                var accountId = _hashingService.DecodeValue(hashedAccountId);
                var legalEntities = await _legalEntitiesService.Get(accountId);
                if (legalEntities.Count() == 1)
                {
                    var apprentices = await _apprenticesService.Get(new ApprenticesQuery(accountId, legalEntities.First().AccountLegalEntityId));
                    if (apprentices.Any())
                    {
                        return RedirectToAction("SelectApprenticeships", new { hashedAccountId });
                    }
                    else
                    {
                        return RedirectToAction("CannotApply", new { hashedAccountId, hasTakenOnNewApprentices = true });
                    }
                }
            }

            return RedirectToAction("CannotApply", new { hashedAccountId });
        }

        public async Task<ViewResult> SelectApprenticeships()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("cannot-apply")]
        public async Task<ViewResult> CannotApply(bool hasTakenOnNewApprentices = false)
        {
            if (hasTakenOnNewApprentices)
            {
                return View(new TakenOnCannotApplyViewModel(_configuration.CommitmentsBaseUrl));
            }
            return View(new CannotApplyViewModel(_configuration.CommitmentsBaseUrl));
        }
    }
}