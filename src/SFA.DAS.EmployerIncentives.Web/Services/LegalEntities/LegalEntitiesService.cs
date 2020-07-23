using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public async Task<IEnumerable<LegalEntityDto>> Get(string accountId)
        {
            var legalEntityList = new List<LegalEntityDto>();

            var id = _hashingService.DecodeValue(accountId);
            using var response = await _client.GetAsync($"/accounts/{id}/legalentities", HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return legalEntityList;
            }

            response.EnsureSuccessStatusCode();

            var result = await JsonSerializer.DeserializeAsync<IEnumerable<LegalEntityDto>>(await response.Content.ReadAsStreamAsync());

            if (!result.Any())
            {
                return legalEntityList;
            }

            result.ToList().ForEach(i => legalEntityList.Add(
                new LegalEntityDto { 
                    AccountId = _hashingService.HashValue(long.Parse(i.AccountId)),
                    AccountLegalEntityId = _hashingService.HashValue(long.Parse(i.AccountLegalEntityId)),
                     LegalEntityName = i.LegalEntityName
                }));

            return legalEntityList;
        }       
    }
}
