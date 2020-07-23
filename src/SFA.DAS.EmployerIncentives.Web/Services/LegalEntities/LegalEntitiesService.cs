using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities
{
    public class LegalEntitiesService : ILegalEntitiesService
    {
        private readonly HttpClient _client;
        private readonly IHashingService _hashingService;
        public LegalEntitiesService(HttpClient client, IHashingService hashingService)
        {
            _client = client;
            _hashingService = hashingService;
        }

        public async Task<IEnumerable<LegalEntityModel>> Get(string accountId)
        {
            var id = _hashingService.DecodeValue(accountId);
            using var response = await _client.GetAsync($"/accounts/{id}/legalentities", HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<LegalEntityModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<LegalEntityDto>>(await response.Content.ReadAsStreamAsync());

            return data.ToLegalEntityModel(_hashingService);
        }       
    }
}
