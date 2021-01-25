using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System;
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
            var url = OuterApiRoutes.LegalEntities.GetLegalEntities(id);

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<LegalEntityModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<LegalEntityDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data.ToLegalEntityModel(_hashingService);
        }

        public async Task<LegalEntityModel> Get(string hashedAccountId, string hashedAccountLegalEntityId)
        {
            try
            {
                var accountLegalEntityId = _hashingService.DecodeValue(hashedAccountLegalEntityId);
                return await Get(hashedAccountId, accountLegalEntityId);
            }
            catch (IndexOutOfRangeException) // hashed id contains invalid characters
            {
                return null;
            }
        }

        public async Task<LegalEntityModel> Get(string hashedAccountId, long accountLegalEntityId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var url = OuterApiRoutes.LegalEntities.GetLegalEntity(accountId, accountLegalEntityId);

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<LegalEntityDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data.ToLegalEntityModel(_hashingService);
        }
    }
}
