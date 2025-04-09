using System;
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
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerIncentives.Web.Authorisation.GovUserEmployerAccount
{
    public class GovAuthEmployerAccountService : IGovAuthEmployerAccountService
    {
        private readonly HttpClient _client;

        public GovAuthEmployerAccountService(HttpClient client)
        {
            _client = client;
        }
        
        
        public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
        {
            var apiResponse = await _client.GetAsync(OuterApiRoutes.UserEmployerAccounts.GetEmployerAccountInfo(userId, email));
            var result = JsonSerializer.Deserialize<GetUserAccountsResponse>(await apiResponse.Content.ReadAsStringAsync());
            return new EmployerUserAccounts
            {
                EmployerAccounts = result.UserAccounts != null? result.UserAccounts.Select(c => new EmployerUserAccountItem
                {
                    Role = c.Role,
                    AccountId = c.AccountId,
                    ApprenticeshipEmployerType = Enum.Parse<ApprenticeshipEmployerType>(c.ApprenticeshipEmployerType.ToString()),
                    EmployerName = c.EmployerName,
                }).ToList() : [],
                FirstName = result.FirstName,
                IsSuspended = result.IsSuspended,
                LastName = result.LastName,
                EmployerUserId = result.EmployerUserId
            };
        }
    }

}