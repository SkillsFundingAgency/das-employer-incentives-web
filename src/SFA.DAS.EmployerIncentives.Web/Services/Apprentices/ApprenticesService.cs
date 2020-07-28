using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;

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
            var queryParams = new Dictionary<string, string>()
            {
                {"accountid", _hashingService.DecodeValue(query.AccountId).ToString()},
                {"accountlegalentityid", _hashingService.DecodeValue(query.AccountLegalEntityId).ToString()}
            };

            var url = QueryHelpers.AddQueryString("/apprenticeships", queryParams);

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<ApprenticeshipModel>();
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeDto>>(
                await response.Content.ReadAsStreamAsync());
            return data.ToApprenticeshipModel(_hashingService);
        }

        public async Task<string> CreateDraftSubmission(CreateDraftSubmission draftSubmission)
        {
            var apprenticeshipIds =
                draftSubmission.ApprenticeshipIds.Select(x => _hashingService.DecodeValue(x)).ToArray();
            var accountId = _hashingService.DecodeValue(draftSubmission.AccountId);
            var accountLegalEntityId = _hashingService.DecodeValue(draftSubmission.AccountLegalEntityId);

            using var response = await _client.PostAsJsonAsync($"/accounts/{accountId}/draft-submissions",
                new {accountLegalEntityId, apprenticeshipIds});

            response.EnsureSuccessStatusCode();

            var result = await JsonSerializer.DeserializeAsync<CreateDraftSubmissionResponse>(
                    await response.Content.ReadAsStreamAsync());
            return _hashingService.HashValue(result.DraftSubmissionId);
        }

        public class CreateDraftSubmissionResponse
        {
            public long AccountId { get; set; }
            public long AccountLegalEntityId { get; set; }
            public long DraftSubmissionId { get; set; }
        }
    }
}