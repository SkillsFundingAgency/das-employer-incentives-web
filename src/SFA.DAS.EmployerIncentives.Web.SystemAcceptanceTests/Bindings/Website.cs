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
                RedisCacheConnectionString = "localhost"
            };
            _context.ExternalLinksOptions = new ExternalLinksConfiguration
            {
                CommitmentsSiteUrl = $"http://{Guid.NewGuid()}",
                ManageApprenticeshipSiteUrl = $"http://{Guid.NewGuid()}",
                EmployerRecruitmentSiteUrl = $"http://{Guid.NewGuid()}"
            };

            _context.Website = new TestWebsite(_context, hook, authHook);
            _context.WebsiteClient = _context.Website.CreateClient();
            _context.HashingService = _context.Website.Services.GetService(typeof(IHashingService)) as IHashingService;
        }
    }
}
