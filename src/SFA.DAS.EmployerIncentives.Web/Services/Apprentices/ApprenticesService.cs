using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public class ApprenticesService : IApprenticesService
    {
        private readonly HttpClient _client;
        private readonly IAccountEncodingService _encodingService;

        public ApprenticesService(HttpClient client, IAccountEncodingService encodingService)
        {
            _client = client;
            _encodingService = encodingService;
        }

        public async Task<IEnumerable<ApprenticeshipModel>> Get(ApprenticesQuery query)
        {
            var url = OuterApiRoutes.Apprenticeships.GetApprenticeships(_encodingService.Decode(query.AccountId),
                _encodingService.Decode(query.AccountLegalEntityId));

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<ApprenticeshipModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeDto>>(
                await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data.ToApprenticeshipModel(_encodingService);
        }
    }
}