namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication
{
    public class TestAuthenticationOptions : ITestAuthenticationOptions
    {
        public TestContext TestContext { get; private set; }

        public TestAuthenticationOptions(TestContext testContext)
        {
            TestContext = testContext;
        }
    }
}
