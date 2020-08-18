using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication;
using System.Collections.Generic;
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
            var claimsIdentity = new ClaimsIdentity(
            new List<Claim>
                {
                    new Claim(EmployerClaimTypes.UserId, TestData.User.AccountOwnerUserId.ToString()),
                    new Claim(EmployerClaimTypes.Account, options.TestContext.TestDataStore.Get<string>("HashedAccountId")),
                    
                    new Claim(EmployerClaimTypes.EmailAddress, "test@test.com"),
                    new Claim(EmployerClaimTypes.GivenName, "FirstName"),
                    new Claim(EmployerClaimTypes.FamilyName, "Surname"),
                    new Claim(EmployerClaimTypes.DisplayName, "Firstname and Surname"),
                    new Claim(EmployerClaimTypes.FamilyName, "Surname")
                },
            "AuthenticationTypes.Federation"
            );

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.User = claimsPrincipal;

            await _next(context);
        }
    }
}
