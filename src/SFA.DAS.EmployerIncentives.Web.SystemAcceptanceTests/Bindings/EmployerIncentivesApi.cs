using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Bindings
{
    [Binding]
    [Scope(Tag = "employerIncentivesApi")]
    public class EmployerIncentivesApi
    {
        private readonly TestContext _context;

        public EmployerIncentivesApi(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario()]
        public void InitialiseApi()
        {
            _context.EmployerIncentivesApi = new TestEmployerIncentivesApi();            
        }
    }
}
