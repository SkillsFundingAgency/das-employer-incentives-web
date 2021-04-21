using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public class ApprenticesService : IApprenticesService
    {
        private readonly HttpClient _client;
        private readonly IHashingService _hashingService;

        public ApprenticesService(HttpClient client, IHashingService hashingService)
        {
            _client = client;
            _hashingService = hashingService;
        }

        public async Task<IEnumerable<ApprenticeshipModel>> Get(ApprenticesQuery query)
        {
            var url = OuterApiRoutes.Apprenticeships.GetApprenticeships(_hashingService.DecodeValue(query.AccountId),
                _hashingService.DecodeValue(query.AccountLegalEntityId), query.PageNumber, query.PageSize);

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<ApprenticeshipModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeDto>>(
                await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data.ToApprenticeshipModel(_hashingService);
        }
    }
}