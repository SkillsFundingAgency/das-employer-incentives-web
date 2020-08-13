using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerIncentives.Web.Authorization;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;        

        public UserService(HttpClient client)
        {
            _client = client;
        }

        public Task<IEnumerable<Claim>> Claims(string userId)
        {
            var accounts = new List<string>
            {
                "MWG69Y",
                "79RBYY",
                "MLB7J9"
            };

            var claims = new List<Claim>();
            var accountsAsJson = System.Text.Json.JsonSerializer.Serialize(accounts.ToDictionary(k => k));
            claims.Add(new Claim(EmployerClaimTypes.Accounts, accountsAsJson, JsonClaimValueTypes.Json));

            return Task.FromResult(claims.AsEnumerable());
        }
    }
}