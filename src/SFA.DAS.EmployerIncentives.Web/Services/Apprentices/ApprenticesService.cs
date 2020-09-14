using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public class ApprenticesService : IApprenticesService
    {
        private readonly HttpClient _client;
        private readonly IHashingService _hashingService;
        private ILogger<ApprenticesService> _logger;

        public ApprenticesService(HttpClient client, IHashingService hashingService, ILogger<ApprenticesService> logger)
        {
            _client = client;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<IEnumerable<ApprenticeshipModel>> Get(ApprenticesQuery query)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"accountid", _hashingService.DecodeValue(query.AccountId).ToString()},
                {"accountlegalentityid", _hashingService.DecodeValue(query.AccountLegalEntityId).ToString()}
            };

            var url = QueryHelpers.AddQueryString("apprenticeships", queryParams);

            _logger.LogInformation($"Retrieving eligible apprenticeships for account {query.AccountId} legal entity {query.AccountLegalEntityId}");

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Apprenticeships not found for account {query.AccountId} legal entity {query.AccountLegalEntityId}");
                return new List<ApprenticeshipModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeDto>>(
                await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _logger.LogInformation($"{data.Count()} eligible apprenticeships found for account {query.AccountId} legal entity {query.AccountLegalEntityId}");

            return data.ToApprenticeshipModel(_hashingService);
        }
    }
}