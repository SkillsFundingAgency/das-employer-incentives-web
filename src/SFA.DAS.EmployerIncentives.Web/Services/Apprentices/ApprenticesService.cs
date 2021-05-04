using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        public async Task<EligibleApprenticeshipsModel> Get(ApprenticesQuery query)
        {
            var data = await GetPagedEligibleApprentices(query.AccountId, query.AccountLegalEntityId, query.PageNumber, query.PageSize);

            var eligibleApprenticeships = new EligibleApprenticesDto 
            {
                PageSize = query.PageSize,
                Apprenticeships = new List<ApprenticeDto>()
            };
            if (query.Offset == 0)
            {
                eligibleApprenticeships.Apprenticeships.AddRange(data.Apprenticeships);
            }
            else
            {
                data.Apprenticeships = data.Apprenticeships.OrderBy(a => a.LastName).ToList();
                for (var apprenticeIndex = (query.Offset - 1); apprenticeIndex < data.Apprenticeships.Count; apprenticeIndex++)
                {
                    eligibleApprenticeships.Apprenticeships.Add(data.Apprenticeships[apprenticeIndex]);
                }
            }

            var pageNumber = query.PageNumber;
            var morePages = false;
            var index = 0;
            while (eligibleApprenticeships.Apprenticeships.Count < data.PageSize 
                   && eligibleApprenticeships.Apprenticeships.Count < data.TotalApprenticeships 
                   && data.Apprenticeships.Count > 0)
            {
                pageNumber++;

                data = await GetPagedEligibleApprentices(query.AccountId, query.AccountLegalEntityId, pageNumber, query.PageSize);
                data.Apprenticeships = data.Apprenticeships.OrderBy(a => a.LastName).ToList();

                while (index < data.Apprenticeships.Count && eligibleApprenticeships.Apprenticeships.Count < query.PageSize)
                {
                    eligibleApprenticeships.Apprenticeships.Add(data.Apprenticeships[index]);
                    index++;
                }
                if (data.Apprenticeships.Count > index)
                {
                    morePages = true;
                }
            }

            if (data.Apprenticeships.Count == query.PageSize && (data.TotalApprenticeships > query.PageSize * query.PageNumber))
            {
                morePages = true;
            }

            eligibleApprenticeships.PageNumber = pageNumber;
            eligibleApprenticeships.MorePages = morePages;
            eligibleApprenticeships.Offset = index + 1;
            eligibleApprenticeships.TotalApprenticeships = data.TotalApprenticeships;

            return eligibleApprenticeships.ToEligibleApprenticeshipsModel(_hashingService);
        }
        
        private async Task<EligibleApprenticesDto> GetPagedEligibleApprentices(string accountId, string accountLegalEntityId, int pageNumber, int pageSize)
        {
            var url = OuterApiRoutes.Apprenticeships.GetApprenticeships(_hashingService.DecodeValue(accountId),
                _hashingService.DecodeValue(accountLegalEntityId), pageNumber, pageSize);

            var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new EligibleApprenticesDto 
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    TotalApprenticeships = 0,
                    Apprenticeships = new List<ApprenticeDto>()
                };
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<EligibleApprenticesDto>(
                await response.Content.ReadAsStreamAsync(),
                options: new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            return data;
        }
    }
}