﻿using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "ApprenticeSelection")]
    public class ApprenticeSelectionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testData;
        private readonly IAccountEncodingService _encodingService;
        private HttpResponseMessage _continueNavigationResponse;
        private List<ApprenticeDto> _apprenticeshipData;
        private LegalEntityDto _legalEntity;

        public ApprenticeSelectionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = _testContext.TestDataStore;
            _encodingService = _testContext.EncodingService;
        }

        [Given(@"an employer applying for a grant has apprentices matching the eligibility requirement")]
        public void GivenAnEmployerApplyingForAGrantHasApprenticesMatchingTheEligibilityRequirement()
        {
            var data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _apprenticeshipData = data.Apprentices;
            _legalEntity = data.LegalEntities.First();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => data.AccountId);
            _testData.Add("HashedAccountId", _encodingService.Encode(accountId));
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _encodingService.Encode(accountId));
            var accountLegalEntityId = _testData.GetOrCreate("AccountLegalEntityId", onCreate: () => data.AccountLegalEntityId);
            _testData.Add("HashedAccountLegalEntityId", _encodingService.Encode(accountLegalEntityId));
            
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", accountId.ToString())
                        .WithParam("accountlegalentityid", accountLegalEntityId.ToString())
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonSerializer.Serialize(_apprenticeshipData, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{accountId}/applications")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Created));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"/accounts/{data.AccountId}/applications/") && !x.Contains("accountlegalentity"))
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonSerializer.Serialize(data.ApplicationResponse, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
               .Given(
                   Request
                       .Create()
                       .WithPath($"/accounts/{accountId}/applications")
                       .UsingPost()
               )
               .RespondWith(
                   Response.Create()
                       .WithStatusCode(HttpStatusCode.Created)
                       .WithHeader("Content-Type", "application/json")
                       .WithBody(string.Empty));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                  Request
                      .Create()
                      .WithPath(x =>
                      x.Contains($"accounts/{data.AccountId}/applications") &&
                      x.Contains("accountlegalentity")) // applicationid is generated in application service so will vary per request
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(data.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{_legalEntity.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonSerializer.Serialize(_legalEntity))
                        .WithStatusCode(HttpStatusCode.OK));
        }

        [When(@"the employer selects the apprentice the grant applies to")]
        public async Task WhenTheEmployerSelectsTheApprenticeTheGrantAppliesTo()
        {
            var apprenticeships = _apprenticeshipData.ToApprenticeshipModel(_encodingService).ToArray();
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/apply/{hashedLegalEntityId}/select-apprentices";
            var form = new KeyValuePair<string, string>("SelectedApprenticeships", apprenticeships.First().Id);

            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url, form);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"the employer doesn't select any apprentice the grant applies to")]
        public async Task WhenTheEmployerDoesnTSelectAnyApprenticeTheGrantAppliesTo()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/apply/{hashedLegalEntityId}/select-apprentices";

            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked to provide employment start dates for the apprentices")]
        public void ThenTheEmployerIsAskedToProvideStartDatesForTheSelectedApprentices()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/{model.ApplicationId}/join-organisation");
            _continueNavigationResponse.Should().HaveBackLink($"/{hashedAccountId}/apply/select-apprentices/{model.ApplicationId}");
            model.Should().HaveTitle($"When did they join {_legalEntity.LegalEntityName}?");
        }

        [Then(@"the employer is informed that they haven't selected an apprentice")]
        public void ThenTheEmployerIsInformedThatTheyHaventSelectedAnApprentice()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            _continueNavigationResponse.Should().HaveTitle(model.Title);
            _continueNavigationResponse.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedLegalEntityId}/select-apprentices");
            model.Should().HaveTitle("Which apprentices do you want to apply for?");
            model.Apprenticeships.Count().Should().Be(_apprenticeshipData.Count);
            model.AccountId.Should().Be(hashedAccountId);
            viewResult.Should().ContainError(model.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
        }
    }
}
