using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.HashingService;
using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Bindings
{
    [Binding]
    public class Website
    {
        private readonly TestContext _context;
        public Website(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 1)]
        public void InitialiseWebsite()
        {
            var hook = new Hook<IActionResult>();
            var authHook = new Hook<AuthorizationHandlerContext>();
            _context.Hooks.Add(hook);
            _context.Hooks.Add(authHook);
            _context.WebConfigurationOptions = new WebConfigurationOptions
            {
                AllowedHashstringCharacters = "46789BCDFGHJKLMNPRSTVWXY",
                Hashstring = "SFA: digital apprenticeship service",
                CommitmentsBaseUrl = $"http://{Guid.NewGuid()}",
                AccountsBaseUrl = $"http://{Guid.NewGuid()}",
                RedisCacheConnectionString = "localhost",
                AchieveServiceBaseUrl = "https://test.achieveservice.com/service/provide-organisation-information",
                DataEncryptionServiceKey = "P5T1NjQ1xqo1FgFM8RG+Yg=="
            };

            _context.Website = new TestWebsite(_context, _context.WebConfigurationOptions, _context.EmployerIncentivesApi, hook, authHook);
            _context.WebsiteClient = _context.Website.CreateClient();
            _context.HashingService = _context.Website.Services.GetService(typeof(IHashingService)) as IHashingService;
        }
    }
}
