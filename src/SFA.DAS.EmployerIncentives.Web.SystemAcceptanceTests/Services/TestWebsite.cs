using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services
{
    public class TestWebsite : WebApplicationFactory<Startup>
    {
        private readonly TestContext _testContext;
        private readonly TestEmployerIncentivesApi _testEmployerIncentivesApi;
        private readonly Dictionary<string, string> _appConfig;
        private readonly IHook<IActionResult> _actionResultHook;
        private readonly WebConfigurationOptions _webConfigurationOptions;
        public IWebHostBuilder WebHostBuilder { get; private set; }

        public TestWebsite(
            TestContext testContext,
            WebConfigurationOptions webConfigurationOptions,
            TestEmployerIncentivesApi testEmployerIncentivesApi, 
            IHook<IActionResult> actionResultHook)
        {
            _testContext = testContext;
            _webConfigurationOptions = webConfigurationOptions;
            _testEmployerIncentivesApi = testEmployerIncentivesApi;
            _actionResultHook = actionResultHook;

            _appConfig = new Dictionary<string, string>
            {
                { "EnvironmentName", "LOCAL" },
                { "Identity:ClientId", "employerincentivesdev" },
                { "Identity:ClientSecret", "secret" },
                { "Identity:BaseAddress", @"https://localhost:8082/identity" },
                { "Identity:Scopes", "openid profile" },
                { "Identity:UsePkce", "false" }
            };
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            WebHostBuilder = builder;

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
                        return new TestAuthenticationOptions(_testContext);
                    });
                    s.AddTransient<IStartupFilter, TestAuthenticationMiddlewareStartupFilter>();                

                    s.Configure<WebConfigurationOptions>(o =>
                    {
                        o.AllowedHashstringCharacters = _webConfigurationOptions.AllowedHashstringCharacters;
                        o.Hashstring = _webConfigurationOptions.Hashstring;
                        o.CommitmentsBaseUrl = _webConfigurationOptions.CommitmentsBaseUrl;
                        o.AccountsBaseUrl = _webConfigurationOptions.AccountsBaseUrl;
                    });
                    s.Configure<EmployerIncentivesApiOptions>(o =>
                      {
                          o.ApiBaseUrl = _testEmployerIncentivesApi.BaseAddress;
                          o.SubscriptionKey = "";
                      });
                    s.Configure<CosmosDbConfigurationOptions>(o =>
                    {
                        o.Uri = "https://localhost:8081/";
                        o.AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                    });                  
                    s.AddControllersWithViews(options =>
                    {
                        options.Filters.Add(new TestActionResultFilter(_actionResultHook));
                    });
                });
        }
    }
}
