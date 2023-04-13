using System.Collections.Generic;
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
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("/")]
    [SetNavigationSection(NavigationSection.AccountsFinance)]
    public class HomeController : Controller
    {
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly IConfiguration _config;
        private readonly IStubAuthenticationService _stubAuthenticationService;
        private readonly ExternalLinksConfiguration _configuration;

        public HomeController(
            ILegalEntitiesService legalEntitiesService,
            IOptions<ExternalLinksConfiguration> configuration,
            IConfiguration config,
            IStubAuthenticationService stubAuthenticationService)
        {
            _legalEntitiesService = legalEntitiesService;
            _config = config;
            _stubAuthenticationService = stubAuthenticationService;
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
            var legalEntity = legalEntities.FirstOrDefault(x => x.AccountLegalEntityId == accountLegalEntityId);
            return View("Home", new HomeViewModel(accountId, accountLegalEntityId, legalEntity?.Name, legalEntity == null || !legalEntity.IsAgreementSigned, _configuration.ManageApprenticeshipSiteUrl, legalEntity == null || legalEntity.BankDetailsRequired));
        }

        [Route("{accountId}/{accountLegalEntityId}/before-you-start")]
        public async Task<IActionResult> BeforeStart(string accountId, string accountLegalEntityId)
        {
            var legalEntities = await _legalEntitiesService.Get(accountId);
            var legalEntity = legalEntities.FirstOrDefault(x => x.AccountLegalEntityId == accountLegalEntityId);            
            return View("BeforeStart", new BeforeYouStartViewModel(accountId, accountLegalEntityId, legalEntity?.Name, legalEntity == null || !legalEntity.IsAgreementSigned, _configuration.ManageApprenticeshipSiteUrl, legalEntity == null || legalEntity.BankDetailsRequired));
        }

        [Route("/signout")]
        [Route("{accountId}/signout")]
        [AllowAnonymous()]
        public new async Task<IActionResult> SignOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = "signoutcleanup",
                AllowRefresh = true
            };
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token",idToken);
            
            return SignOut(
                authenticationProperties,
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Route("signoutcleanup")]
        [AllowAnonymous()]
        public async Task SignOutCleanup()
        {
            Response.Cookies.Delete(CookieNames.AuthCookie);
        }
        
#if DEBUG
        [AllowAnonymous()]
        [HttpGet]
        [Route("SignIn-Stub")]
        public IActionResult SigninStub()
        {
            return View("SigninStub", new List<string>{_config["StubId"],_config["StubEmail"]});
        }
        
        [AllowAnonymous()]
        [HttpPost]
        [Route("SignIn-Stub")]
        public IActionResult SigninStubPost()
        {
            _stubAuthenticationService?.AddStubEmployerAuth(Response.Cookies, new StubAuthUserDetails
            {
                Email = _config["StubEmail"],
                Id = _config["StubId"]
            });

            return RedirectToRoute("Signed-in-stub");
        }

        [Authorize("StubAuthentication")]
        [HttpGet]
        [Route("signed-in-stub", Name = "Signed-in-stub")]
        public IActionResult SignedInStub()
        {
            return View();
        }
#endif
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously