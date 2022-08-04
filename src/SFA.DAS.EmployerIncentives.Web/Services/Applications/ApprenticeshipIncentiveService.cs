using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class ApprenticeshipIncentiveService : IApprenticeshipIncentiveService
    {
        private readonly HttpClient _client;
        private readonly IHashingService _hashingService;

        public ApprenticeshipIncentiveService(HttpClient client, IHashingService hashingService)
        {
            _client = client;
            _hashingService = hashingService;
        }

        public async Task Cancel(string accountLegalEntityId, IEnumerable<ApprenticeshipIncentiveModel> apprenticeshipIncentives,
            string hashedAccountId, string emailAddress)
        {
            var decodedAccountLegalEntityId = _hashingService.DecodeValue(accountLegalEntityId);

            var url = $"withdrawals";
            var serviceRequest = new ServiceRequest()
            {
                TaskId = Guid.NewGuid().ToString(),
                TaskCreatedDate = DateTime.UtcNow
            };

            var applications = apprenticeshipIncentives.Select(apprenticeshipIncentive => new Application { AccountLegalEntityId = decodedAccountLegalEntityId, ULN = apprenticeshipIncentive.Uln }).ToList();
            
            var request = new WithdrawRequest(
                WithdrawalType.Employer,
                applications,
                serviceRequest,
                _hashingService.DecodeValue(hashedAccountId),
                emailAddress
                );

            using var response = await _client.PostAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<ApprenticeshipIncentiveModel>> GetList(string accountId, string accountLegalEntityId)
        {
            var decodedAccountId = _hashingService.DecodeValue(accountId);
            var decodedAccountLegalEntityId = _hashingService.DecodeValue(accountLegalEntityId);
            using var response = await _client.GetAsync($"accounts/{decodedAccountId}/legalentities/{decodedAccountLegalEntityId}/apprenticeshipIncentives", HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<ApprenticeshipIncentiveModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeshipIncentiveModel>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }
    }
}
