using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities
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
            var queryParams = new Dictionary<string, string>()
            {
                {"accountid", _hashingService.DecodeValue(query.AccountId).ToString() },
                {"accountlegalentityid", _hashingService.DecodeValue(query.AccountLegalEntityId).ToString() }
            };
            
            var url = QueryHelpers.AddQueryString("/apprenticeships", queryParams);
                                                    
            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<ApprenticeshipModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeDto>>(await response.Content.ReadAsStreamAsync());
            return data.ToApprenticeshipModel(_hashingService);
        }
    }
}
