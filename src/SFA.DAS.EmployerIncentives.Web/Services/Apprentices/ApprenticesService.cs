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
        private readonly IPaginationService _paginationService;

        public ApprenticesService(HttpClient client, IHashingService hashingService, IPaginationService paginationService)
        {
            _client = client;
            _hashingService = hashingService;
            _paginationService = paginationService;
        }

        public async Task<EligibleApprenticeshipsModel> Get(ApprenticesQuery query)
        {
            var pagingInformation = await _paginationService.GetPagingInformation(query);
    
            var data = await GetPagedEligibleApprentices(query.AccountId, query.AccountLegalEntityId, pagingInformation.PageNumber, query.PageSize);
            var startIndex = query.StartIndex;
            var eligibleApprenticeships = new EligibleApprenticesDto 
            {
                PageSize = query.PageSize,
                Apprenticeships = new List<ApprenticeDto>()
            };
            if (pagingInformation.Offset == 0)
            {
                eligibleApprenticeships.Apprenticeships.AddRange(data.Apprenticeships);
            }
            else
            {
                data.Apprenticeships = data.Apprenticeships.OrderBy(a => a.DisplayName).ToList();
                for (var apprenticeIndex = pagingInformation.Offset; apprenticeIndex < data.Apprenticeships.Count; apprenticeIndex++)
                {
                    eligibleApprenticeships.Apprenticeships.Add(data.Apprenticeships[apprenticeIndex]);
                }
            }

            var pageNumber = pagingInformation.PageNumber;
            var morePages = false;
            var offset = 0;
            while (eligibleApprenticeships.Apprenticeships.Count < data.PageSize 
                   && (eligibleApprenticeships.Apprenticeships.Count + ((pageNumber - 1) * query.PageSize) < data.TotalApprenticeships )
                       && eligibleApprenticeships.Apprenticeships.Count < data.TotalApprenticeships)
            {
                var index = 0;
                pageNumber++;

                data = await GetPagedEligibleApprentices(query.AccountId, query.AccountLegalEntityId, pageNumber, query.PageSize);
                data.Apprenticeships = data.Apprenticeships.OrderBy(a => a.DisplayName).ToList();

                while (eligibleApprenticeships.Apprenticeships.Count < query.PageSize)
                {
                    if (index >= data.Apprenticeships.Count)
                    {
                        break;
                    }
                    eligibleApprenticeships.Apprenticeships.Add(data.Apprenticeships[index]);
                    index++;
                }
                if (index < query.PageSize)
                {
                    offset = index;
                }
                if (data.Apprenticeships.Count > index)
                {
                    morePages = true;
                }
            }

            if (data.TotalApprenticeships > query.PageSize * pageNumber)
            {
                morePages = true;
            }

            if (data.Apprenticeships.Count == query.PageSize)
            {
                pageNumber++;
            }

            var nextPageInformation = new PagingInformation
            { 
                Offset = offset, 
                PageNumber = pageNumber, 
                StartIndex = query.StartIndex + query.PageSize
            };

            await _paginationService.SavePagingInformation(nextPageInformation);
            
            if (nextPageInformation.Offset == 0)
            {
                var nextPageData = await GetPagedEligibleApprentices(query.AccountId, query.AccountLegalEntityId, nextPageInformation.PageNumber, query.PageSize);
                if (nextPageData.Apprenticeships.Count == 0
                    && nextPageInformation.PageNumber * query.PageSize >= nextPageData.TotalApprenticeships)
                {
                    morePages = false;
                }
            }

            eligibleApprenticeships.MorePages = morePages;
            eligibleApprenticeships.Offset = offset;
            eligibleApprenticeships.TotalApprenticeships = data.TotalApprenticeships;
            eligibleApprenticeships.StartIndex = startIndex;
            
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