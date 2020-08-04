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
        private readonly WebConfigurationOptions _webConfigurationOptions;

        public TestWebsite(
            WebConfigurationOptions webConfigurationOptions,
            TestEmployerIncentivesApi testEmployerIncentivesApi, 
            IHook<IActionResult> actionResultHook)
        {
            _webConfigurationOptions = webConfigurationOptions;
            _testEmployerIncentivesApi = testEmployerIncentivesApi;
            _actionResultHook = actionResultHook;

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
                    s.Configure<WebConfigurationOptions>(o =>
                    {
                        o.AllowedHashstringCharacters = _webConfigurationOptions.AllowedHashstringCharacters;
                        o.Hashstring = _webConfigurationOptions.Hashstring;
                        o.CommitmentsBaseUrl = _webConfigurationOptions.CommitmentsBaseUrl;
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
