using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using System.Collections.Generic;
namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services
{
    public class TestWebsite : WebApplicationFactory<Startup>
    {
        private readonly TestEmployerIncentivesApi _testEmployerIncentivesApi;
        private readonly Dictionary<string, string> _appConfig;
        private readonly IHook<IActionResult> _actionResultHook;

        public TestWebsite(TestEmployerIncentivesApi testEmployerIncentivesApi, IHook<IActionResult> actionResultHook)
        {
            _testEmployerIncentivesApi = testEmployerIncentivesApi;
            _actionResultHook = actionResultHook;

            _appConfig = new Dictionary<string, string>
            {
                { "Environment", "LOCAL" },
                { "ConfigurationStorageConnectionString", "UseDevelopmentStorage=true" },
                { "ConfigNames", "SFA.DAS.EmployerIncentives.Web" },
                { "Values:AzureWebJobsStorage", "UseDevelopmentStorage=true" }
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
                    s.Configure<WebConfigurationOptions>(o =>
                    {
                        o.AllowedHashstringCharacters = "46789BCDFGHJKLMNPRSTVWXY";
                        o.Hashstring = "SFA: digital apprenticeship service";
                    });
                    s.Configure<EmployerIncentivesApiOptions>(o =>
                      {
                          o.ApiBaseUrl = _testEmployerIncentivesApi.BaseAddress;
                          o.SubscriptionKey = "";
                      });
                    s.AddControllersWithViews(options =>
                    {
                        options.Filters.Add(new TestActionResultFilter(_actionResultHook));
                    });
                });
        }
    }
}
