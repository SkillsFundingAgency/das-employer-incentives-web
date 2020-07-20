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

        public LegalEntitiesService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<LegalEntityDto>> Get(long accountId)
        {
            using var response = await _client.GetAsync($"/accounts/{accountId}/legalentities", HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<LegalEntityDto>();
            }

            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<IEnumerable<LegalEntityDto>>(await response.Content.ReadAsStreamAsync());
        }
    }
}
