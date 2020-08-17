using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.RouteValues;
using SFA.DAS.EmployerIncentives.Web.Services.Users;
using SFA.DAS.HashingService;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Authorisation
{
    public class EmployerAccountAuthorizationHandler : AuthorizationHandler<EmployerAccountRequirement>
    {
        private readonly IUserService _userService;
        private readonly IHashingService _hashingService;

        public EmployerAccountAuthorizationHandler(
            IUserService userService, 
            IHashingService hashingService)
        {
            _userService = userService;
            _hashingService = hashingService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountRequirement requirement)
        {
            var isAuthorised = await IsEmployerAuthorised(context);
            if (isAuthorised)
            {
                context.Succeed(requirement);
            }
        }

        public Task<bool> IsEmployerAuthorised(AuthorizationHandlerContext context)
        {   
            if (!(context.Resource is AuthorizationFilterContext mvcContext) || !mvcContext.RouteData.Values.ContainsKey(RouteValueKeys.AccountHashedId))
            {
                return Task.FromResult(false);
            }

            var accountIdFromUrl = mvcContext.RouteData.Values[RouteValueKeys.AccountHashedId].ToString().ToUpper();
            var userIdClaim = context.User.FindFirst(c => c.Type.Equals(EmployerClaimTypes.UserId));
            if (userIdClaim?.Value == null)
            {
                return Task.FromResult(false);
            }

            if(!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Task.FromResult(false);
            }

            var accountClaim = context.User.FindFirst(c => 
                c.Type.Equals(EmployerClaimTypes.Account) &&
                c.Value.Equals(accountIdFromUrl, StringComparison.InvariantCultureIgnoreCase)
                );

            if (accountClaim?.Value != null)
            {
                return Task.FromResult(true);
            }
            
            return Task.FromResult(false);
        }
    }
}
