using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
{
    public class TestContext : IDisposable
    {
        public DirectoryInfo TestDirectory { get; set; }
        public TestWebsite Website { get; set; }
        public HttpClient WebsiteClient { get; set; }
        public TestEmployerIncentivesApi EmployerIncentivesApi { get; set; }
        public IHashingService HashingService { get; set; }
        public TestDataStore TestDataStore { get; set; }
        public List<IHook> Hooks { get; set; }
        public TestActionResult ActionResult { get; set; }
        public WebConfigurationOptions WebConfigurationOptions { get; set; }

        private bool _isDisposed;

        public TestContext()
        {
            TestDirectory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString()));
            if (!TestDirectory.Exists)
            {
                Directory.CreateDirectory(TestDirectory.FullName);
            }
            TestDataStore = new TestDataStore();
            Hooks = new List<IHook>();            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                WebsiteClient?.Dispose();
                EmployerIncentivesApi?.Dispose();
            }

            _isDisposed = true;
        }
    }
}


