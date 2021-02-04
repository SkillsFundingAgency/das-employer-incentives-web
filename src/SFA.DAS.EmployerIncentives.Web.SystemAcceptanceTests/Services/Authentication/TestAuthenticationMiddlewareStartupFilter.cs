using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication
{
    public class TestAuthenticationMiddlewareStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<TestAuthenticationMiddlewareForAchieveService>();
                builder.UseMiddleware<TestAuthenticationMiddleware>();                
                next(builder);
            };
        }
    }
}
