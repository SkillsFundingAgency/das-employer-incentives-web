using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
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

            if (_context.WebConfigurationOptions == null)
            {
                _context.WebConfigurationOptions = new WebConfigurationOptions();
            }

            _context.WebConfigurationOptions.AllowedHashstringCharacters = "46789BCDFGHJKLMNPRSTVWXY";
            _context.WebConfigurationOptions.Hashstring = "SFA: digital apprenticeship service";
            _context.WebConfigurationOptions.RedisCacheConnectionString = "localhost";
            _context.WebConfigurationOptions.AchieveServiceBaseUrl = "https://test.achieveservice.com/service/provide-organisation-information";
            _context.WebConfigurationOptions.DataEncryptionServiceKey = "P5T1NjQ1xqo1FgFM8RG+Yg==";
            _context.WebConfigurationOptions.EmploymentCheckErrorMessages = new Dictionary<string, string>
            {
                {"NinoNotFound", "Check and update National Insurance number"},
                {"PAYENotFound", "Check and update PAYE scheme"},
                {"NinoAndPAYENotFound", "Check and update PAYE scheme and National Insurance number"}
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
