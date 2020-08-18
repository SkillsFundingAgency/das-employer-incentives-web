using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication
{
    public interface ITestAuthenticationOptions
    {
        List<Claim> Claims { get; }
    }
}
