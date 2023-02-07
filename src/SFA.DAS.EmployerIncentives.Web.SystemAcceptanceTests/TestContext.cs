using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
{
    public class TestContext : IDisposable
    {
        public DirectoryInfo TestDirectory { get; set; }
        public TestWebsite Website { get; set; }
        public TestCosmosDb ReadStore { get; set; }        
        public HttpClient WebsiteClient { get; set; }
        public TestEmployerIncentivesApi EmployerIncentivesApi { get; set; }
        public IAccountEncodingService EncodingService { get; set; }
        public TestDataStore TestDataStore { get; set; }
        public List<IHook> Hooks { get; set; }
        public List<Claim> Claims { get; set; }
        public TestActionResult ActionResult { get; set; }
        public WebConfigurationOptions WebConfigurationOptions { get; set; }
        public ExternalLinksConfiguration ExternalLinksOptions { get; set; }
        public CosmosDbConfigurationOptions CosmosDbConfigurationOptions { get; set; }
        
        private bool _isDisposed;

        public void AddOrReplaceClaim(string type, string value)
        {
            var existing = Claims.SingleOrDefault(c => c.Type == type);
            if(existing != null)
            {
                Claims.Remove(existing);
            }
            Claims.Add(new Claim(type, value));
        }

        public TestContext()
        {
            TestDirectory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString()));
            if (!TestDirectory.Exists)
            {
                Directory.CreateDirectory(TestDirectory.FullName);
            }
            TestDataStore = new TestDataStore();
            Hooks = new List<IHook>();

            Claims = new List<Claim>
                {
                    new Claim(EmployerClaimTypes.UserId, TestData.User.AccountOwnerUserId.ToString()),
                    new Claim(EmployerClaimTypes.Account, TestData.User.AuthenticatedHashedId),
                    new Claim(EmployerClaimTypes.EmailAddress, "test@test.com"),
                    new Claim(EmployerClaimTypes.GivenName, "FirstName"),
                    new Claim(EmployerClaimTypes.FamilyName, "Surname"),
                    new Claim(EmployerClaimTypes.DisplayName, "Firstname and Surname"),
                    new Claim(EmployerClaimTypes.FamilyName, "Surname")
                };
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


