﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using System;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
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

            _context.WebConfigurationOptions.RedisCacheConnectionString = "localhost";
            _context.WebConfigurationOptions.AchieveServiceBaseUrl = "https://test.achieveservice.com/service/provide-organisation-information";
            _context.WebConfigurationOptions.DataEncryptionServiceKey = "P5T1NjQ1xqo1FgFM8RG+Yg==";
            if (String.IsNullOrWhiteSpace(_context.WebConfigurationOptions.ApplicationShutterPageDate)) // can already be set by shutter page hook so don't override
            {
                _context.WebConfigurationOptions.ApplicationShutterPageDate = DateTime.Today.AddDays(1).ToString("dd MMM yyyy");
            }

            _context.ExternalLinksOptions = new ExternalLinksConfiguration
            {
                CommitmentsSiteUrl = $"http://{Guid.NewGuid()}",
                ManageApprenticeshipSiteUrl = $"http://{Guid.NewGuid()}",
                EmployerRecruitmentSiteUrl = $"http://{Guid.NewGuid()}"
            };

            _context.Website = new TestWebsite(_context, hook, authHook);
            _context.WebsiteClient = _context.Website.CreateClient();
            _context.EncodingService = _context.Website.Services.GetService(typeof(IAccountEncodingService)) as IAccountEncodingService;
        }
    }
}
