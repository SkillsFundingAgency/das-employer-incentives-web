using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication
{
    public class TestAuthenticationMiddleware
    {   
        private readonly RequestDelegate _next;

        public TestAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITestAuthenticationOptions options)
        {
            if (options.Claims?.Count > 0)
            {                
                var claimsIdentity = new ClaimsIdentity(
                    options.Claims,
                    "AuthenticationTypes.Federation"
                );

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;
            }

            await _next(context);
        }
    }
}
