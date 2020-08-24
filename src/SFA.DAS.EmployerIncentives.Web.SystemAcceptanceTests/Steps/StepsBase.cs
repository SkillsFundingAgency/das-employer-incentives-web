using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using System.Linq;

[assembly: Parallelizable(ParallelScope.Fixtures)]
namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps
{
    public class StepsBase
    {
        protected readonly TestContext TestContext;

        public StepsBase(TestContext testContext)
        {
            TestContext = testContext;
            var hook = testContext.Hooks.SingleOrDefault(h => h is Hook<IActionResult>) as Hook<IActionResult>;

            testContext.ActionResult = new Services.TestActionResult();
            hook.OnProcessed = (message) => { testContext.ActionResult.SetActionResult(message); };
            hook.OnErrored = (ex, message) => { testContext.ActionResult.SetException(ex); };
        }
    }
}
