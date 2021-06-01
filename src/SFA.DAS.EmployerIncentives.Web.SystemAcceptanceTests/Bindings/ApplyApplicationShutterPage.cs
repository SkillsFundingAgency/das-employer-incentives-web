using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Bindings
{
    [Binding]
    [Scope(Tag = "applyApplicationShutterPage")]
    public class ApplyApplicationShutterPage
    {
        private readonly TestContext _context;
        public ApplyApplicationShutterPage(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 0)]
        public void InitialiseWebsite()
        {
            if(_context.WebConfigurationOptions == null)
            {
                _context.WebConfigurationOptions = new WebConfigurationOptions();
            }
            _context.WebConfigurationOptions.ApplicationShutterPageDate = DateTime.Today.ToString("dd MMM yyyy");
        }
    }
}
