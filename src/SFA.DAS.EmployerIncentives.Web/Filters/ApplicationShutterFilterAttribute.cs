using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using System;

namespace SFA.DAS.EmployerIncentives.Web.Filters
{
    public class ApplicationShutterFilterAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private DateTime _applyFromDate;
        private bool _isInitialised = false;

        public ApplicationShutterFilterAttribute(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            if (!_isInitialised)
            {
                Initialise();
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

        private void Initialise()
        {
            var webOptions = new WebConfigurationOptions();
            _configuration.GetSection(WebConfigurationOptions.EmployerIncentivesWebConfiguration).Bind(webOptions);
            DateTime.TryParse(webOptions.ApplicationShutterPageDate, out _applyFromDate);
            _isInitialised = true;
        }
    }
}