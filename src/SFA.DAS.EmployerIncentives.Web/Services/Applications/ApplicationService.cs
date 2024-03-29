﻿using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Exceptions;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class ApplicationService : IApplicationService
    {
        private readonly HttpClient _client;
        private readonly IAccountEncodingService _encodingService;

        public ApplicationService(HttpClient client, IAccountEncodingService encodingService)
        {
            _client = client;
            _encodingService = encodingService;
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

        public async Task<ApplicationModel> Get(string accountId, Guid applicationId, bool includeApprenticeships = true, bool includeSubmitted = false)
        {
            var url = OuterApiRoutes.Application.GetApplication(_encodingService.Decode(accountId), applicationId, includeApprenticeships);
            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<ApplicationResponse>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if(!includeSubmitted && !string.IsNullOrEmpty(data.Application.SubmittedByEmail))
            {
                return null;
            }
            
            return MapFromGetApplicationResponse(data.Application, accountId, applicationId);
        }

        public async Task<GetApplicationsModel> GetList(string accountId, string accountLegalEntityId)
        {
            var decodedAccountId = _encodingService.Decode(accountId);
            var decodedAccountLegalEntityId = _encodingService.Decode(accountLegalEntityId);
            using var response = await _client.GetAsync($"accounts/{decodedAccountId}/legalentity/{decodedAccountLegalEntityId}/applications", HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new GetApplicationsModel
                {
                    BankDetailsStatus = BankDetailsStatus.NotSupplied,
                    ApprenticeApplications = new List<ApprenticeApplicationModel>()
                };
            }

            response.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<GetApplicationsModel>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
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

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new UlnAlreadySubmittedException();
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task<long> GetApplicationLegalEntity(string accountId, Guid applicationId)
        {
            var url = OuterApiRoutes.Application.GetApplicationLegalEntity(_encodingService.Decode(accountId), applicationId);
            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var accountLegalEntityId = await JsonSerializer.DeserializeAsync<long>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return accountLegalEntityId;
        }

        public async Task SaveApprenticeshipDetails(ApprenticeshipDetailsRequest request)
        {
            var url = OuterApiRoutes.Application.SaveApprenticeshipDetails(request.AccountId, request.ApplicationId);
            using var response = await _client.PatchAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();
        }

        private ApplicationModel MapFromGetApplicationResponse(IncentiveApplicationDto application, string accountId, Guid applicationId)
        {
            return new ApplicationModel(applicationId, accountId,
                _encodingService.Encode(application.AccountLegalEntityId),
                application.Apprenticeships.OrderBy(x => x.LastName).Select(MapFromApplicationApprenticeDto),
                application.BankDetailsRequired, application.NewAgreementRequired);
        }

        private ApplicationApprenticeshipModel MapFromApplicationApprenticeDto(IncentiveApplicationApprenticeshipDto apprentice)
        {
            return new ApplicationApprenticeshipModel
            {
                ApprenticeshipId = _encodingService.Encode(apprentice.ApprenticeshipId),
                CourseName = apprentice.CourseName,
                FirstName = apprentice.FirstName,
                LastName = apprentice.LastName,
                ExpectedAmount = apprentice.TotalIncentiveAmount,
                StartDate = apprentice.PlannedStartDate,
                Uln = apprentice.Uln,
                EmploymentStartDate = apprentice.EmploymentStartDate,
                StartDatesAreEligible = apprentice.StartDatesAreEligible
            };
        }

        private CreateApplicationRequest MapToCreateApplicationRequest(Guid applicationId, string accountId, string accountLegalEntityId, IEnumerable<string> apprenticeshipIds)
        {
            return new CreateApplicationRequest(applicationId, _encodingService.Decode(accountId),
                _encodingService.Decode(accountLegalEntityId),
                apprenticeshipIds.Select(x => _encodingService.Decode(x)));
        }

        private UpdateApplicationRequest MapToUpdateApplicationRequest(Guid applicationId, string accountId, IEnumerable<string> apprenticeshipIds)
        {
            return new UpdateApplicationRequest
            {
                ApplicationId = applicationId,
                AccountId = _encodingService.Decode(accountId),
                ApprenticeshipIds = apprenticeshipIds.Select(x => _encodingService.Decode(x)).ToArray()
            };
        }

        private ConfirmApplicationRequest MapToConfirmApplicationRequest(Guid applicationId, string accountId, string userEmail, string userName)
        {
            return new ConfirmApplicationRequest(applicationId, _encodingService.Decode(accountId), userEmail, userName);
        }

    }
}