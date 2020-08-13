using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.RouteValues;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Authorization
{
    public class EmployerAccountAuthorizationHandler : AuthorizationHandler<EmployerAccountRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountRequirement requirement)
        {
            if (IsEmployerAuthorised(context))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        public bool IsEmployerAuthorised(AuthorizationHandlerContext context)
        {
            if (!(context.Resource is AuthorizationFilterContext mvcContext) || !mvcContext.RouteData.Values.ContainsKey(RouteValueKeys.AccountHashedId))
            {
                return false;
            }

            var accountIdFromUrl = mvcContext.RouteData.Values[RouteValueKeys.AccountHashedId].ToString().ToUpper();
            var employerAccountClaim = context.User.FindFirst(c => c.Type.Equals(EmployerClaimTypes.Accounts));

            if (employerAccountClaim?.Value == null)
            {
                return false;
            }

            Dictionary<string, string> employerAccounts;

            try
            {
                employerAccounts = JsonSerializer.Deserialize<Dictionary<string, string>>(employerAccountClaim.Value);
            }
            catch (Exception)
            {
                return false;
            }

            if (employerAccounts.ContainsKey(accountIdFromUrl))
            {
                return true;
            }

            return false;
        }
    }
}
