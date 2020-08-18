using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Bindings
{
    [Binding]
    public class CosmosDb
    {
        private readonly TestContext _context;
        public CosmosDb(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 10)]
        public void InitialiseCosmosDb()
        {
            _context.CosmosDbConfigurationOptions = new CosmosDbConfigurationOptions
            {
                Uri = "http://localhost:8081/",
                AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            };

            _context.ReadStore = new TestCosmosDb(_context.Website.WebHostBuilder, _context.CosmosDbConfigurationOptions);
        }
    }
}
