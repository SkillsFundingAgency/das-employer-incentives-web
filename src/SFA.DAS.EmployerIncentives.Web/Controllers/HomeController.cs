using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly ExternalLinksConfiguration _configuration;
        private const int NewAgreementVersion = 5;

        public HomeController(ILegalEntitiesService legalEntitiesService, IOptions<ExternalLinksConfiguration> configuration)
        {
            _legalEntitiesService = legalEntitiesService;
            _configuration = configuration.Value;
        }

        [Route("")]            
        [AllowAnonymous()]
        public async Task<IActionResult> AnonymousHome()
        {            
            return RedirectToAction("login");
        }

        [Route("/login")]        
        public async Task<IActionResult> Login()
        {
            if (User.HasClaim(c => c.Type.Equals(EmployerClaimTypes.Account)))
            {
                return RedirectToAction("Home", new { accountId = User.Claims.First(c => c.Type.Equals(EmployerClaimTypes.Account)).Value });
            }
            return Forbid();
        }

        [Route("{accountId}")]
        public async Task<IActionResult> Home(string accountId)
        {
            return RedirectToAction("GetChooseOrganisation", "ApplyOrganisation");
        }

        [Route("{accountId}/{accountLegalEntityId}")]
        public async Task<IActionResult> Start(string accountId, string accountLegalEntityId)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);
            var hasMultipleLegalEntities = legalEntities.Count() > 1;
            var legalEntity = legalEntities.FirstOrDefault(x => x.AccountLegalEntityId == accountLegalEntityId);
            var newAgreementRequired = (legalEntity != null && legalEntity.SignedAgreementVersion.HasValue && legalEntity.SignedAgreementVersion != NewAgreementVersion);
            return View("Home", new HomeViewModel(accountId, accountLegalEntityId, legalEntity?.Name, hasMultipleLegalEntities, newAgreementRequired, _configuration.ManageApprenticeshipSiteUrl));
        
        }

        [Route("/signout")]
        [Route("{accountId}/signout")]
        [AllowAnonymous()]
        public async Task<IActionResult> SignOut()
        {
            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    RedirectUri = "signoutcleanup",
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Route("signoutcleanup")]
        [AllowAnonymous()]
        public async Task SignOutCleanup()
        {
            Response.Cookies.Delete(CookieNames.AuthCookie);
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously