using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.Services.Users.Types;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerIncentives.Web.Authorisation.GovUserEmployerAccount
{
    public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
    {
        private readonly HttpClient _client;

        public EmployerAccountPostAuthenticationClaimsHandler(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var userId = tokenValidatedContext.Principal.Claims
                    .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
                    .Value;
            var email = tokenValidatedContext.Principal.Claims
                .First(c => c.Type.Equals(ClaimTypes.Email))
                .Value;
            
            var apiResponse = await _client.GetAsync(OuterApiRoutes.UserEmployerAccounts.GetEmployerAccountInfo(userId, email));
            var result = JsonSerializer.Deserialize<GetUserAccountsResponse>(await apiResponse.Content.ReadAsStringAsync());
            
            var claims = new List<Claim>();

            if (result?.UserAccounts == null)
            {
                return claims;
            }
            
            result.UserAccounts
                .Where(c => c.Role.Equals(UserRole.Owner.ToString()) || c.Role.Equals(UserRole.Transactor.ToString()))
                .ToList().ForEach(u => claims.Add(new Claim(EmployerClaimTypes.Account, u.AccountId)));
            claims.Add(new Claim(EmployerClaimTypes.UserId, result.EmployerUserId));
            claims.Add(new Claim(EmployerClaimTypes.GivenName, result.FirstName));
            claims.Add(new Claim(EmployerClaimTypes.FamilyName, result.LastName));
            return claims;
        }
    }

}