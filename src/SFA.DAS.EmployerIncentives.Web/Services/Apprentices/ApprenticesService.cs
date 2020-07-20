using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.LegalEntities
{
    public class ApprenticesService : IApprenticesService
    {
        private readonly HttpClient _client;

        public ApprenticesService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ApprenticeDto>> Get(ApprenticesQuery query)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"accountid", query.AccountId.ToString() },
                {"accountlegalentityid", query.AccountLegalEntityId.ToString() }
            };
            
            var url = QueryHelpers.AddQueryString("/apprenticeships", queryParams);
                                                    
            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<ApprenticeDto>();
            }

            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeDto>>(await response.Content.ReadAsStreamAsync());
        }
    }
}
