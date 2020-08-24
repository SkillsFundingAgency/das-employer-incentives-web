using Microsoft.AspNetCore.Mvc;
using System;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services
{
    public class TestActionResult
    {
        public IActionResult LastActionResult { get; private set; }
        public ViewResult LastViewResult { get; private set; }
        public Exception LastException { get; private set; }

        public TestActionResult()
        {
            LastActionResult = null;
            LastViewResult = null;
            LastException = null;
        }

        public void SetActionResult(IActionResult actionResult)
        {
            LastActionResult = actionResult;
            if(actionResult is ViewResult)
            {
                LastViewResult = actionResult as ViewResult;
            }
        }

        public void SetException(Exception exception)
        {
            LastException = exception;
        }
    }
}
