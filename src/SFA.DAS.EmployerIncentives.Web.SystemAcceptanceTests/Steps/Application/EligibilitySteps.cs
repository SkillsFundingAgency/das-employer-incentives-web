﻿using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
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
    [Scope(Feature = "Eligibility")]
    public class EligibilitySteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;

        public EligibilitySteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
        }

        [Given(@"an employer applying for a grant has eligible apprenticeships")]
        [Given(@"an employer with a single organisation applying for a grant has eligible apprenticeships")]
        public void GivenAnEmployerApplyingHasASingleLegalEntityWithEligibleApprenticeships()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);
            _testDataStore.Add("LegalEntity", testdata.LegalEntity);

            _testContext.EmployerIncentivesApi.MockServer
           .Given(
                   Request
                   .Create()
                   .WithPath($"/accounts/{testdata.AccountId}/legalentities")
                   .UsingGet()
                   )
               .RespondWith(
             Response.Create()
               .WithStatusCode(HttpStatusCode.OK)
               .WithBody(JsonSerializer.Serialize(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{testdata.AccountId}/legalentities/{testdata.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonSerializer.Serialize(testdata.LegalEntities.First(), TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
             .Given(
                     Request
                     .Create()
                     .WithPath($"/apprenticeships")
                     .WithParam("accountid", testdata.AccountId.ToString())
                     .WithParam("accountlegalentityid", testdata.LegalEntities.First().AccountLegalEntityId.ToString())
                     .UsingGet()
                     )
                 .RespondWith(
             Response.Create()
                 .WithBody(JsonSerializer.Serialize(testdata.Apprentices, TestHelper.DefaultSerialiserSettings))
                 .WithStatusCode(HttpStatusCode.OK));
        }

        [Given(@"an employer with a single organisation applying for a grant has no eligible apprenticeships")]
        public void GivenAnEmployerApplyingHasASingleLegalEntityWithNoEligibleApprenticeships()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithNoEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);
            _testDataStore.Add("LegalEntity", testdata.LegalEntity);

            _testContext.EmployerIncentivesApi.MockServer
           .Given(
                   Request
                   .Create()
                   .WithPath($"/accounts/{testdata.AccountId}/legalentities")
                   .UsingGet()
                   )
               .RespondWith(
             Response.Create()
               .WithStatusCode(HttpStatusCode.OK)
               .WithBody(JsonSerializer.Serialize(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{testdata.AccountId}/legalentities/{testdata.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonSerializer.Serialize(testdata.LegalEntities.First(), TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
             .Given(
                     Request
                     .Create()
                     .WithPath($"/apprenticeships")
                     .WithParam("accountid", testdata.AccountId.ToString())
                     .WithParam("accountlegalentityid", testdata.LegalEntities.First().AccountLegalEntityId.ToString())
                     .UsingGet()
                     )
                 .RespondWith(
             Response.Create()
                 .WithStatusCode(HttpStatusCode.NotFound));
        }

        [Given(@"an employer applying for a grant does not have eligible apprenticeships")]
        public void GivenAnEmployerApplyingDoesNotHaveEligibleApprenticeships()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithNoEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);
            _testDataStore.Add("LegalEntity", testdata.LegalEntity);

            _testContext.EmployerIncentivesApi.MockServer
            .Given(
                    Request
                    .Create()
                    .WithPath($"/accounts/{testdata.AccountId}/legalentities")
                    .UsingGet()
                    )
                .RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(JsonSerializer.Serialize(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{testdata.AccountId}/legalentities/{testdata.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonSerializer.Serialize(testdata.LegalEntities.First(), TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/apprenticeships")
                      .WithParam("accountid", testdata.AccountId.ToString())
                      .WithParam("accountlegalentityid", testdata.LegalEntity.AccountLegalEntityId.ToString())
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithStatusCode(HttpStatusCode.NotFound));
        }

        [When(@"the employer specifies that they have eligible apprenticeships")]
        public async Task WhenTheEmployerSelectsThatTheyHaveEligibleApprenticeships()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{hashedAccountId}/apply/{hashedAccountLegalEntityId}/eligible-apprentices")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("HasTakenOnNewApprentices", "true")
                })
            };

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
        }

        [When(@"the employer specifies that they do not have eligible apprenticeships")]
        public async Task WhenTheEmployerSelectsThatTheyDoNotHaveEligibleApprenticeships()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{hashedAccountId}/apply/{hashedAccountLegalEntityId}/eligible-apprentices")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("HasTakenOnNewApprentices", "false")
                })
            };

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
        }

        [When(@"the employer does not specify whether or not they have eligible apprenticeships")]
        public async Task WhenTheEmployerDoesNotSelectAnEligibleApprenticeshipsoption()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{hashedAccountId}/apply/{hashedAccountLegalEntityId}/eligible-apprentices")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("OrganisationName", testdata.LegalEntities.First().LegalEntityName)
                })
            };

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
        }

        [Then(@"the employer is asked to select the apprenticeship")]
        public void ThenTheEmployerIsAskedWoSelectTheApprenticeship()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Which apprentices do you want to apply for?");
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountLegalEntityId.Should().Be(hashedAccountLegalEntityId);

            response.Should().HaveTitle(model.Title);
            response.Should().HaveBackLink($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/eligible-apprentices");
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/select-apprentices");
        }

        [Then(@"the employer is informed that they cannot apply")]
        public void ThenTheEmployerIsInformedTheyCannotApply()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var legalEntity = _testDataStore.Get<LegalEntityDto>("LegalEntity");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as TakenOnCannotApplyViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle($"{legalEntity.LegalEntityName} cannot apply for this payment");
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountHomeUrl.Should().Be($"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{hashedAccountId}/teams");

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedLegalEntityId}/no-new-apprentices");
        }

        [Then(@"the employer is informed that they cannot apply yet")]
        public void ThenTheEmployerIsInformedTheyCannotApplyYet()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var legalEntity = _testDataStore.Get<LegalEntityDto>("LegalEntity");

            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as CannotApplyViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle($"{legalEntity.LegalEntityName} cannot apply for this payment");
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountHomeUrl.Should().Be($"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{hashedAccountId}/teams");
            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedLegalEntityId}/cannot-apply");
        }

        [Then(@"the employer is informed that they need to specify whether or not they have eligible apprenticeships")]
        public void ThenTheEmployerIsInformedTheyNeedToSelectAnOption()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var legalEntity = _testDataStore.Get<LegalEntityDto>("LegalEntity");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as QualificationQuestionViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle($"Eligible apprentices at {legalEntity.LegalEntityName}");
            model.AccountId.Should().Be(hashedAccountId);

            response.Should().HaveTitle(model.Title);
            response.Should().HaveBackLink($"/{hashedAccountId}/{hashedAccountLegalEntityId}");
        }
    }
}
