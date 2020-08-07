using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class ApplicationService : IApplicationService
    {
        private readonly HttpClient _client;
        private readonly IHashingService _hashingService;

        public ApplicationService(HttpClient client, IHashingService hashingService)
        {
            _client = client;
            _hashingService = hashingService;
        }

        public async Task<Guid> Create(string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds)
        {
            var applicationId = Guid.NewGuid();
            var request = MapToPostRequest(applicationId, accountId, accountLegalEntityId, apprenticeshipIds);

            using var response = await _client.PostAsJsonAsync($"accounts/{request.AccountId}/applications", request);

            response.EnsureSuccessStatusCode();

            return applicationId;
        }

        public async Task<ApplicationConfirmationViewModel> Get(string accountId, Guid applicationId)
        {
            using var response = await _client.GetAsync($"accounts/{_hashingService.DecodeValue(accountId)}/applications/{applicationId}", HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<ApplicationResponse>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return MapFromGetApplicationResponse(data.Application, accountId, applicationId);
        }

        private ApplicationConfirmationViewModel MapFromGetApplicationResponse(IncentiveApplicationDto application, string accountId, Guid applicationId)
        {
            return new ApplicationConfirmationViewModel(applicationId, accountId,
                _hashingService.HashValue(application.AccountLegalEntityId),
                application.Apprenticeships.Select(MapFromApplicationApprenticeDto));
        }

        private ApplicationConfirmationViewModel.ApplicationApprenticeship MapFromApplicationApprenticeDto(IncentiveApplicationApprenticeshipDto apprentice)
        {
            return new ApplicationConfirmationViewModel.ApplicationApprenticeship
            {
                ApprenticeshipId = _hashingService.HashValue(apprentice.ApprenticeshipId),
                CourseName = apprentice.CourseName,
                FirstName = apprentice.FirstName,
                LastName = apprentice.LastName,
                ExpectedAmount = apprentice.TotalIncentiveAmount
            };
        }

        private CreateApplicationRequest MapToPostRequest(Guid applicationId, string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds)
        {
            return new CreateApplicationRequest(applicationId, _hashingService.DecodeValue(accountId),
                _hashingService.DecodeValue(accountLegalEntityId),
                apprenticeshipIds.Select(x => _hashingService.DecodeValue(x)));
        }
    }
}