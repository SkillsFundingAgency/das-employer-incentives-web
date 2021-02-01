using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            _logger.LogInformation("[HomeController] Logged in with AccountId: {accountId}", accountId);
            _logger.LogInformation("[HomeController] Logged in with AccountId: {AccountId}", new { AccountId = accountId });

            return View(new HomeViewModel(accountId));
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