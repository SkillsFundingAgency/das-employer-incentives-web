using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Filters
{
    public class GoogleAnalyticsFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            controller.ViewBag.GaData = PopulateForEmployer(context);

            base.OnActionExecuting(context);
        }

        private GoogleAnalyticsData PopulateForEmployer(ActionExecutingContext context)
        {
            string hashedAccountId = null;

            var userId = context.HttpContext.User.FindFirst(c => c.Type.Equals(EmployerClaimTypes.UserId))?.Value;

            if (context.RouteData.Values.TryGetValue("employerAccountId", out var employerAccountId))
            {
                hashedAccountId = employerAccountId.ToString();
            }

            return new GoogleAnalyticsData
            {
                UserId = userId,
                Acc = hashedAccountId
            };
        }

        public string DataLoaded { get; set; }
    }
}