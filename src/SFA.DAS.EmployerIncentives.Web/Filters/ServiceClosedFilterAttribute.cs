using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.EmployerIncentives.Web.Filters
{
    public class ServiceClosedFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var routeDataDictionary = controller.ControllerContext.RouteData?.Values;

            if (
                (routeDataDictionary?["controller"]?.ToString() == "ApplicationComplete") ||
                (routeDataDictionary?["controller"]?.ToString() == "ApplyApprenticeships") ||
                (routeDataDictionary?["controller"]?.ToString() == "Apply" && routeDataDictionary?["action"]?.ToString() != "Forbidden") ||
                (routeDataDictionary?["controller"]?.ToString() == "ApplyEmploymentDetails") ||
                (routeDataDictionary?["controller"]?.ToString() == "ApplyOrganisation") ||
                (routeDataDictionary?["controller"]?.ToString() == "ApplyQualification") ||
                (routeDataDictionary?["controller"]?.ToString() == "Home" && routeDataDictionary?["action"]?.ToString() == "Start") ||
                (routeDataDictionary?["controller"]?.ToString() == "Home" && routeDataDictionary?["action"]?.ToString() == "BeforeStart") ||
                (routeDataDictionary?["controller"]?.ToString() == "Home" && routeDataDictionary?["action"]?.ToString() == "Home") ||
                (routeDataDictionary?["controller"]?.ToString() == "Hub") ||
                (routeDataDictionary?["controller"]?.ToString() == "Payments") ||
                (routeDataDictionary?["controller"]?.ToString() == "BankDetails") ||
                (routeDataDictionary?["controller"]?.ToString() == "Cancel") ||
                (routeDataDictionary?["controller"]?.ToString() == "System" && routeDataDictionary?["action"]?.ToString() == "ApplicationsClosed") ||
                (routeDataDictionary?["controller"]?.ToString() == "Error" && routeDataDictionary?["action"]?.ToString() == "PageNotFound")
            )
            {
                context.Result = new RedirectResult($"/{routeDataDictionary?["accountId"]?.ToString()}/service-closed");
            }


            base.OnActionExecuting(context);
        }

    }
}