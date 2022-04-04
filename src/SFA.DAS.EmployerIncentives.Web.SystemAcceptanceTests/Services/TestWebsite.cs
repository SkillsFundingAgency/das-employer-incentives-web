using Microsoft.AspNetCore.Authorization;
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
        private readonly Dictionary<string, string> _appConfig;
        private readonly IHook<IActionResult> _actionResultHook;
        private readonly IHook<AuthorizationHandlerContext> _authContextHook;
        public IWebHostBuilder WebHostBuilder { get; private set; }

        public TestWebsite(
            TestContext testContext,
            IHook<IActionResult> actionResultHook,
            IHook<AuthorizationHandlerContext> authContextHook)
        {
            _testContext = testContext;
            _actionResultHook = actionResultHook;
            _authContextHook = authContextHook;

            _appConfig = new Dictionary<string, string>
            {
                { "EnvironmentName", "LOCAL" },
                { "Identity:ClientId", "employerincentivesdev" },
                { "Identity:ClientSecret", "secret" },
                { "Identity:BaseAddress", @"https://localhost:8082/identity" },
                { "Identity:Scopes", "openid profile" },
                { "Identity:UsePkce", "false" }
            };

            if(!string.IsNullOrEmpty(_testContext.WebConfigurationOptions?.ApplicationShutterPageDate))
            {
                _appConfig.Add("EmployerIncentivesWeb:ApplicationShutterPageDate", _testContext.WebConfigurationOptions?.ApplicationShutterPageDate);
            }            
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
                        return new TestAuthenticationOptions(_testContext.Claims);
                    });
                    s.AddTransient<IStartupFilter, TestAuthenticationMiddlewareStartupFilter>();

                    s.Configure<WebConfigurationOptions>(o =>
                    {
                        o.AllowedHashstringCharacters = _testContext.WebConfigurationOptions.AllowedHashstringCharacters;
                        o.Hashstring = _testContext.WebConfigurationOptions.Hashstring;
                        o.AchieveServiceBaseUrl = _testContext.WebConfigurationOptions.AchieveServiceBaseUrl;
                        o.DataEncryptionServiceKey = _testContext.WebConfigurationOptions.DataEncryptionServiceKey;
                        o.ApplicationShutterPageDate = _testContext.WebConfigurationOptions.ApplicationShutterPageDate;
                        o.EmploymentCheckErrorMessages = _testContext.WebConfigurationOptions.EmploymentCheckErrorMessages;
                    });
                    s.Configure<ExternalLinksConfiguration>(o =>
                    {
                        o.ManageApprenticeshipSiteUrl = _testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl;
                        o.CommitmentsSiteUrl = _testContext.ExternalLinksOptions.CommitmentsSiteUrl;
                        o.EmployerRecruitmentSiteUrl = _testContext.ExternalLinksOptions.EmployerRecruitmentSiteUrl;
                    });
                    s.Configure<EmployerIncentivesApiOptions>(o =>
                      {
                          o.ApiBaseUrl = _testContext.EmployerIncentivesApi.BaseAddress;
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

                    s.Decorate<IAuthorizationHandler>((handler, testhandler) =>
                    {
                        return new TestAuthorizationHandler(handler, _authContextHook);
                    });
                });
        }
    }
}
