using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [Route("")]            
        [AllowAnonymous()]
        public IActionResult AnonymousHome()
        {            
            return RedirectToAction("login");
        }

        [Route("/login")]        
        public IActionResult Login()
        {
            if (User.HasClaim(c => c.Type.Equals(EmployerClaimTypes.Account)))
            {
                return RedirectToAction("Home", new { accountId = User.Claims.First(c => c.Type.Equals(EmployerClaimTypes.Account)).Value });
            }
            return Forbid();
        }

        [Route("{accountId}")]
        public IActionResult Home(string accountId)
        {
            return View(new HomeViewModel(accountId));
        }

        [Route("{accountId}/signout")]
        public IActionResult SignOut()
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
        public void SignOutCleanup()
        {
            Response.Cookies.Delete(CookieNames.AuthCookie);
        }
    }
}