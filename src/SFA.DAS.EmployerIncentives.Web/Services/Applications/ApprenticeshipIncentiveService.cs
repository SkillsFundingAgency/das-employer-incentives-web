using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;
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

        public async Task<GetApplicationsModel> GetList(string accountId, string accountLegalEntityId)
        {
            var decodedAccountId = _hashingService.DecodeValue(accountId);
            var decodedAccountLegalEntityId = _hashingService.DecodeValue(accountLegalEntityId);
            using var response = await _client.GetAsync($"accounts/{decodedAccountId}/legalentity/{decodedAccountLegalEntityId}/applications", HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new GetApplicationsModel
                {
                    BankDetailsStatus = BankDetailsStatus.NotSupplied,
                    ApprenticeApplications = new List<ApprenticeApplicationModel>()
                };
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<GetApplicationsModel>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }
    }
}
