using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerIncentives.Web.RouteValues;
using SFA.DAS.HashingService;
using System;

namespace SFA.DAS.EmployerIncentives.Web.Authorization
{
    //public class AuthorizationContextProvider : IAuthorizationContextProvider
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly IHashingService _hashingService;

    //    public AuthorizationContextProvider(IHttpContextAccessor httpContextAccessor,
    //        IHashingService hashingService)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //        _hashingService = hashingService;
    //    }

    //    public IAuthorizationContext GetAuthorizationContext()
    //    {
    //        var authorizationContext = new AuthorizationContext();
    //        var (hashedAccountId, accountId) = GetAccountValues();
    //        var (userId, email) = GetUserValues();

    //        if (accountId.HasValue)
    //        {
    //            authorizationContext.AddEmployerUserRoleValues(accountId.Value, userId.Value);
    //        }
            
    //        return authorizationContext;
    //    }

    //    private (string hashedAccountId, long? accountId) GetAccountValues()
    //    {
    //        if (!_httpContextAccessor.HttpContext.GetRouteData().Values.TryGetValue(RouteValueKeys.AccountHashedId, out var hashedAccountId))
    //        {
    //            return (null, null);
    //        }

    //        if (!_hashingService.TryDecodeValue(hashedAccountId.ToString(), out var accountId))
    //        {
    //            throw new UnauthorizedAccessException();
    //        }

    //        return (hashedAccountId.ToString(), accountId);
    //    }

    //    private (Guid? userId, string email) GetUserValues()
    //    {
    //        var userClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id");

    //        if (userClaim == null)
    //        {
    //            throw new UnauthorizedAccessException();
    //        }

    //        if (!Guid.TryParse(userClaim.Value, out var userId))
    //        {
    //            throw new UnauthorizedAccessException();
    //        }

    //        return (userId, "userEmail");
    //    }
    //}
}
