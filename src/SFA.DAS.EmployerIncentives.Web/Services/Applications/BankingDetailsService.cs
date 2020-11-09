using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class BankingDetailsService : IBankingDetailsService
    {
        private readonly HttpClient _client;

        public BankingDetailsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<BankingDetailsDto> GetBankingDetails(long accountId, Guid applicationId, string hashedAccountId)
        {
            var url = OuterApiRoutes.Application.GetBankingDetailsUrl(accountId, applicationId, hashedAccountId);

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<BankingDetailsDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }
    }
}
