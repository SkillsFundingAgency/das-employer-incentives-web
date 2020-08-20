using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.EmployerIncentives.Web.MockServer
{
    public class LocalWebSite : WebApplicationFactory<Startup>
    {
        private readonly List<Claim> _claims;
        private readonly Dictionary<string, string> _appConfig;

        public LocalWebSite(List<Claim> claims)
        {
            _claims = claims;
            
            _appConfig = new Dictionary<string, string>
            {
                { "EnvironmentName", "LOCAL" }
            };

        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
               .ConfigureAppConfiguration(a =>
               {
                   a.Sources.Clear();
                   a.AddInMemoryCollection(_appConfig);
               });

            builder
                .ConfigureServices(s =>
                {
                    s.AddTransient<TestAuthenticationMiddleware>();
                    s.AddScoped<ITestAuthenticationOptions, TestAuthenticationOptions>(s =>
                    {
                        return new TestAuthenticationOptions(_claims);
                    });
                });

            base.ConfigureWebHost(builder);
        }
    }
}
