using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;

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
            object accountIdRouteValue;
            object applicationIdRouteValue;
            
            string hashedAccountId = null;
            string hashedAccountLegalEntityId = null;

            var userId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier))?.Value;

            if (context.RouteData.Values.TryGetValue("accountId", out accountIdRouteValue))
            {
                if (!accountIdRouteValue.ToString().Contains(".html"))
                {
                    hashedAccountId = accountIdRouteValue.ToString();
                }
            }
            
            if (context.RouteData.Values.TryGetValue("accountId", out accountIdRouteValue)
                && context.RouteData.Values.TryGetValue("applicationId", out applicationIdRouteValue))
            {
                hashedAccountLegalEntityId = GetAccountLegalEntityIdFromApplication(context, accountIdRouteValue, applicationIdRouteValue);
            }
            else if (context.RouteData.Values.TryGetValue("accountLegalEntityId", out var accountLegalEntityId))
            {
                hashedAccountLegalEntityId = accountLegalEntityId.ToString();
            }

            return new GoogleAnalyticsData
            {
                UserId = userId,
                Acc = hashedAccountId,
                Ale = hashedAccountLegalEntityId
            };
        }

        private static string GetAccountLegalEntityIdFromApplication(ActionExecutingContext context, object accountIdRouteValue, object applicationIdRouteValue)
        {
            string hashedAccountLegalEntityId;
            var applicationService = context.HttpContext.RequestServices.GetService<IApplicationService>();
            var applicationId = new Guid(applicationIdRouteValue.ToString());
            var accountLegalEntityId = applicationService.GetApplicationLegalEntity(accountIdRouteValue.ToString(), applicationId).GetAwaiter().GetResult();

            var hashingService = context.HttpContext.RequestServices.GetService<IHashingService>();
            hashedAccountLegalEntityId = hashingService.HashValue(accountLegalEntityId.ToString());
            return hashedAccountLegalEntityId;
        }

        public string DataLoaded { get; set; }

    }
}