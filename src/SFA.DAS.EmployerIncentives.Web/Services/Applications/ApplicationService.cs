﻿using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
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
            var request = MapToCreateApplicationRequest(applicationId, accountId, accountLegalEntityId, apprenticeshipIds);

            var url = OuterApiRoutes.Application.CreateApplication(request.AccountId);
            using var response = await _client.PostAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();

            return applicationId;
        }

        public async Task<ApplicationConfirmationViewModel> Get(string accountId, Guid applicationId, bool includeApprenticeships = true)
        {
            var url = OuterApiRoutes.Application.GetApplication(_hashingService.DecodeValue(accountId), applicationId, includeApprenticeships);
            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<ApplicationResponse>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return MapFromGetApplicationResponse(data.Application, accountId, applicationId);
        }

        public async Task Update(Guid applicationId, string accountId, IEnumerable<string> apprenticeshipIds)
        {
            var request = MapToUpdateApplicationRequest(applicationId, accountId, apprenticeshipIds);

            var url = OuterApiRoutes.Application.UpdateApplication(request.AccountId);
            using var response = await _client.PutAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();
        }

        public async Task Confirm(string accountId, Guid applicationId, string userEmail, string userName)
        {
            var request = MapToConfirmApplicationRequest(applicationId, accountId, userEmail, userName);

            var url = OuterApiRoutes.Application.ConfirmApplication(request.AccountId);
            using var response = await _client.PatchAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();
        }

        public async Task<long> GetApplicationLegalEntity(string accountId, Guid applicationId)
        {
            var url = OuterApiRoutes.Application.GetApplicationLegalEntity(_hashingService.DecodeValue(accountId), applicationId);
            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var accountLegalEntityId = await JsonSerializer.DeserializeAsync<long>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return accountLegalEntityId;
        }

        public async Task<IEnumerable<ApprenticeApplicationModel>> GetList(string accountId, string accountLegalEntityId)
        {
            var decodedAccountId = _hashingService.DecodeValue(accountId);
            var decodedAccountLegalEntityId = _hashingService.DecodeValue(accountLegalEntityId);
            using var response = await _client.GetAsync($"accounts/{decodedAccountId}/legalentity/{decodedAccountLegalEntityId}/applications", HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<IEnumerable<ApprenticeApplicationModel>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }

        private ApplicationConfirmationViewModel MapFromGetApplicationResponse(IncentiveApplicationDto application, string accountId, Guid applicationId)
        {
            return new ApplicationConfirmationViewModel(applicationId, accountId,
                _hashingService.HashValue(application.AccountLegalEntityId),
                application.Apprenticeships.OrderBy(x => x.LastName).Select(MapFromApplicationApprenticeDto),
                application.BankDetailsRequired);
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

        private CreateApplicationRequest MapToCreateApplicationRequest(Guid applicationId, string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds)
        {
            return new CreateApplicationRequest(applicationId, _hashingService.DecodeValue(accountId),
                _hashingService.DecodeValue(accountLegalEntityId),
                apprenticeshipIds.Select(x => _hashingService.DecodeValue(x)));
        }

        private UpdateApplicationRequest MapToUpdateApplicationRequest(Guid applicationId, string accountId, IEnumerable<string> apprenticeshipIds)
        {
            return new UpdateApplicationRequest
            {
                ApplicationId = applicationId,
                AccountId = _hashingService.DecodeValue(accountId),
                ApprenticeshipIds = apprenticeshipIds.Select(x => _hashingService.DecodeValue(x)).ToArray()
            };
        }

        private ConfirmApplicationRequest MapToConfirmApplicationRequest(Guid applicationId, string accountId, string userEmail, string userName)
        {
            return new ConfirmApplicationRequest(applicationId, _hashingService.DecodeValue(accountId), userEmail, userName);
        }
    }
}