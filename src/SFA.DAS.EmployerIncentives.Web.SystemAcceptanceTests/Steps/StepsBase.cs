using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps
{
    public class StepsBase
    {
        protected readonly TestContext TestContext;

        public StepsBase(TestContext testContext)
        {
            TestContext = testContext;
        }
    }
}
