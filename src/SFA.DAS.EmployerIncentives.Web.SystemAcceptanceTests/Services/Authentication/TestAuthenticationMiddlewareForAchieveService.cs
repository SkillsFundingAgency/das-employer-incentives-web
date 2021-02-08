using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication
{
    public class TestAuthenticationMiddlewareForAchieveService
    {
        private readonly RequestDelegate _next;

        public TestAuthenticationMiddlewareForAchieveService(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITestAuthenticationOptions options)
        {
            if (context.Request.Host.ToString() != "test.achieveservice.com" || options.Claims?.Count == 0)
            {
                await _next(context);
                return;
            }

            var accountClaim = options.Claims.SingleOrDefault(c => c.Type == EmployerClaimTypes.Account);

            options.Claims.Remove(accountClaim);
            options.Claims.Add(new Claim(EmployerClaimTypes.Account, "SERVICE"));
            
            await _next(context);

            options.Claims.Remove(options.Claims.SingleOrDefault(c => c.Type == EmployerClaimTypes.Account));
            options.Claims.Add(accountClaim);
        }
    }
}
