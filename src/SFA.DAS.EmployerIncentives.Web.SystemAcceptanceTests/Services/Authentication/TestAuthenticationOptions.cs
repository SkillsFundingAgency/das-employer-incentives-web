using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication
{
    public class TestAuthenticationOptions : ITestAuthenticationOptions
    {
        public List<Claim> Claims { get; private set; }

        public TestAuthenticationOptions(List<Claim> claims)
        {
            Claims = claims;
        }
    }
}
