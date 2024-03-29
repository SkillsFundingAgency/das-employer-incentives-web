﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

            var encodingConfig = File.ReadAllText(Path.Combine(testContext.TestDirectory.Parent.ToString(), "local.encoding.json"));

            _appConfig = new Dictionary<string, string>
            {
                { "EnvironmentName", "LOCAL_ACCEPTANCE_TESTS" },
                { "Identity:ClientId", "employerincentivesdev" },
                { "Identity:ClientSecret", "secret" },
                { "Identity:BaseAddress", @"https://localhost:8082/identity" },
                { "Identity:Scopes", "openid profile" },
                { "Identity:UsePkce", "false" },
                { "SFA.DAS.Encoding", encodingConfig }
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
                        o.AchieveServiceBaseUrl = _testContext.WebConfigurationOptions.AchieveServiceBaseUrl;
                        o.DataEncryptionServiceKey = _testContext.WebConfigurationOptions.DataEncryptionServiceKey;
                        o.ApplicationShutterPageDate = _testContext.WebConfigurationOptions.ApplicationShutterPageDate;
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
