using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;

#pragma warning disable S3881 // "IDisposable" should be implemented correctly
namespace SFA.DAS.EmployerIncentives.Web.MockServer
{
    public class LocalWebSite : IDisposable
    {
        private readonly List<Claim> _claims;
        private IWebHost _host;
        private readonly Dictionary<string, string> _appConfig;

        public LocalWebSite(List<Claim> claims)
        {
            _claims = claims;
            
            _appConfig = new Dictionary<string, string>
            {
                { "EnvironmentName", "LOCAL" }
            };
        }

        public LocalWebSite Run()
        {
            _host.Run();
            return this;
        }

        public LocalWebSite Build()
        {
            var builder = WebHost
                .CreateDefaultBuilder(null)
                .UseStartup<Startup>();

            builder
               .ConfigureAppConfiguration(a =>
               {
                   a.Sources.Clear();
                   a.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            { "EnvironmentName", "LOCAL" }
                        });
                   a.AddJsonFile("appsettings.development.json");
               });

            builder
                .ConfigureServices(s =>
                {
                    s.AddTransient<TestAuthenticationMiddleware>();
                    s.AddScoped<ITestAuthenticationOptions, TestAuthenticationOptions>(s =>
                    {
                        return new TestAuthenticationOptions(_claims);
                    });
                    s.AddTransient<IStartupFilter, TestAuthenticationMiddlewareStartupFilter>();
                });

            _host = builder.Build();
            
            return this;
        }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
#pragma warning restore S3881 // "IDisposable" should be implemented correctly