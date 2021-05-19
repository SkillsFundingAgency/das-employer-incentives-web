using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using System;

namespace SFA.DAS.EmployerIncentives.Web.Filters
{
    public class ApplicationShutterFilterAttribute : ActionFilterAttribute
    {
        private readonly DateTime _applyFromDate;
        public ApplicationShutterFilterAttribute(WebConfigurationOptions options)
        {
            DateTime.TryParse(options.ApplicationShutterPageDate, out _applyFromDate);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            if (_applyFromDate != DateTime.MinValue && DateTime.Today >= _applyFromDate.Date)
            {
                var routeDataDictionary = controller.ControllerContext.RouteData?.Values;

                if ((routeDataDictionary?["controller"]?.ToString() == "Home" && routeDataDictionary?["action"]?.ToString() == "Start") ||
                    (routeDataDictionary?["controller"]?.ToString() == "Apply" && routeDataDictionary?["action"]?.ToString() == "SubmitApplication"))
                {
                    context.Result = new RedirectResult($"/{routeDataDictionary?["accountId"]?.ToString()}/system-update");
                }
            }

             base.OnActionExecuting(context);
        }
    }
}