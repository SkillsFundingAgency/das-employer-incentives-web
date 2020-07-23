﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using System.Collections.Generic;
namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services
{
    public class TestWebsite : WebApplicationFactory<Startup>
    {
        private readonly TestEmployerIncentivesApi _testEmployerIncentivesApi;
        private readonly Dictionary<string, string> _appConfig;

        public TestWebsite(TestEmployerIncentivesApi testEmployerIncentivesApi)
        {
            _testEmployerIncentivesApi = testEmployerIncentivesApi;

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
                });
        }
    }
}
