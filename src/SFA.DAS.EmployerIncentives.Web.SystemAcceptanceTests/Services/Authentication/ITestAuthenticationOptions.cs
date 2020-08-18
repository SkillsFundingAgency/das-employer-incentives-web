namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services.Authentication
{
    public interface ITestAuthenticationOptions
    {
        TestContext TestContext { get; } // might be better to have a collection of claims here
    }
}
