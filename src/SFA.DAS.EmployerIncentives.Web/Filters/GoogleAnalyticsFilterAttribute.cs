﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

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
            
            string hashedAccountLegalEntityId = null;

            var userId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaimTypes.UserId))?.Value;
            var hashedAccountId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaimTypes.Account))?.Value;
                                
            if (context.RouteData.Values.TryGetValue("accountLegalEntityId", out var accountLegalEntityId))
            {
                hashedAccountLegalEntityId = accountLegalEntityId.ToString();
            }
            else if (context.RouteData.Values.TryGetValue("accountId", out accountIdRouteValue)
                && context.RouteData.Values.TryGetValue("applicationId", out applicationIdRouteValue))
            {
                hashedAccountLegalEntityId = GetAccountLegalEntityIdFromApplication(context, accountIdRouteValue, applicationIdRouteValue);
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

            var encodingService = context.HttpContext.RequestServices.GetService<IAccountEncodingService>();
            hashedAccountLegalEntityId = encodingService.Encode(accountLegalEntityId);
            return hashedAccountLegalEntityId;
        }

        public string DataLoaded { get; set; }

    }
}