using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public BankingDetailsService(HttpClient client, ILoggerFactory logger)
        {
            _client = client;
            _logger = logger.CreateLogger(typeof(ILogger));
        }

        public async Task<BankingDetailsDto> GetBankingDetails(long accountId, Guid applicationId, string hashedAccountId)
        {
            var url = OuterApiRoutes.GetBankingDetailsUrl(accountId, applicationId, hashedAccountId);

            _logger.LogInformation("[BankingDetailsService] Call outer API: " + url);

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            _logger.LogInformation("[BankingDetailsService] Response status code: " + response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogError("[BankingDetailsService] Error occurred: " + response.ReasonPhrase);
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<BankingDetailsDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }
    }
}
