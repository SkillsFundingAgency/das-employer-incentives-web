using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

            var data = await JsonSerializer.DeserializeAsync<GetApplicationResponse>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return MapFromGetApplicationResponse(data);
        }

        public async Task Confirm(string accountId, Guid applicationId)
        {
            const string user = "TestUserId"; // TODO: Use authenticated user https://skillsfundingagency.atlassian.net/browse/EI-191
            var request = MapToConfirmApplicationRequest(applicationId, accountId, user);

            using var response = await _client.PatchAsJsonAsync($"/accounts/{request.AccountId}/applications/{applicationId}", request);

            response.EnsureSuccessStatusCode();
        }

        private ApplicationConfirmationViewModel MapFromGetApplicationResponse(GetApplicationResponse application)
        {
            return new ApplicationConfirmationViewModel(application.ApplicationId, _hashingService.HashValue(application.AccountId),
                _hashingService.HashValue(application.AccountLegalEntityId),
                application.Apprentices.Select(MapFromApplicationApprenticeDto));
        }

        private ApplicationConfirmationViewModel.ApplicationApprenticeship MapFromApplicationApprenticeDto(ApplicationApprenticeshipDto apprentice)
        {
            return new ApplicationConfirmationViewModel.ApplicationApprenticeship
            {
                ApprenticeshipId = _hashingService.HashValue(apprentice.ApprenticeshipId),
                CourseName = apprentice.CourseName,
                FirstName = apprentice.FirstName,
                LastName = apprentice.LastName,
                ExpectedAmount = apprentice.ExpectedAmount
            };
        }

        private CreateApplicationRequest MapToPostRequest(Guid applicationId, string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds)
        {
            return new CreateApplicationRequest(applicationId, _hashingService.DecodeValue(accountId),
                _hashingService.DecodeValue(accountLegalEntityId),
                apprenticeshipIds.Select(x => _hashingService.DecodeValue(x)));
        }

        private ConfirmApplicationRequest MapToConfirmApplicationRequest(Guid applicationId, string accountId, string user)
        {
            return new ConfirmApplicationRequest(applicationId, _hashingService.DecodeValue(accountId), user);
        }
    }
}