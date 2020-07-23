﻿using AngleSharp.Html.Parser;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "ApplicationEligibility")]
    public class EligibilitySteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testData;
        private readonly IHashingService _hashingService;

        public EligibilitySteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = _testContext.TestDataStore;
            _hashingService = _testContext.HashingService;
        }

        [Given(@"an employer applying for a grant has no apprenticeships matching the eligibility requirement")]
        public void GivenAnEmployerApplyingHasNoApprenticesMatchingTheEligibilityCriteria()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithNoEligibleApprenticeships();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testData.Add("HashedAccountId", _hashingService.HashValue(accountId));
            var accountLegalEntityId = _testData.GetOrCreate("AccountLegalEntityId", onCreate: () => testdata.AccountLegalEntityId);
            _testData.Add("HashedAccountLegalEntityId", _hashingService.HashValue(accountLegalEntityId));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/accounts/{accountId}/legalentities")
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities))
                  .WithStatusCode(HttpStatusCode.OK));

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
                  .WithStatusCode(HttpStatusCode.NotFound));
        }

        [Given(@"an employer applying for a grant has apprenticeships matching the eligibility requirement")]
        public void GivenAnEmployerApplyingHasApprenticesMatchingTheEligibilityCriteria()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testData.Add("HashedAccountId", _hashingService.HashValue(accountId));
            var accountLegalEntityId = _testData.GetOrCreate("AccountLegalEntityId", onCreate: () => testdata.AccountLegalEntityId);
            _testData.Add("HashedAccountLegalEntityId", _hashingService.HashValue(accountLegalEntityId));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/accounts/{accountId}/legalentities")
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithStatusCode(HttpStatusCode.OK)
                  .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities)));

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
                  .WithBody(JsonConvert.SerializeObject(testdata.Apprentices))
                  .WithStatusCode(HttpStatusCode.OK));
        }

        [Given(@"an employer applying for a grant has no legal entities")]
        public void GivenAnEmployerApplyingHasNoLegalentities()
        {
            var testdata = new TestData.Account.WithNoLegalEntites();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testData.Add("HashedAccountId", _hashingService.HashValue(accountId));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/accounts/{accountId}/legalentities")
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithStatusCode(HttpStatusCode.NotFound));
        }

        [Given(@"an employer applying for a grant has multiple legal entities")]
        public void GivenAnEmployerApplyingHasMultipleLegalentities()
        {
            var testdata = new TestData.Account.WithMultipleLegalEntities();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testData.Add("HashedAccountId", _hashingService.HashValue(accountId));

            var legalEntities = _testData.GetOrCreate("Legalentities", onCreate: () => testdata.LegalEntities);

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/accounts/{accountId}/legalentities")
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithStatusCode(HttpStatusCode.OK)
                  .WithBody(JsonConvert.SerializeObject(legalEntities)));
        }

        [When(@"the employer tries to make a grant application")]
        public async Task WhenTheEmployerMakesAGrantApplication()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get, 
                $"{_testData.Get<string>("HashedAccountId")}/apply")
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("hasTakenOnNewApprentices", "true")
                    })
                };            

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }

        [Then(@"the employer is informed they cannot apply for the grant yet")]
        public async Task ThenTheEmployerIsInformedTheycannotApplyForTheGrantYet()
        {
            _testContext.ActionResult.LastViewResult.Should().NotBeNull();
            var model = _testContext.ActionResult.LastViewResult.Model as CannotApplyViewModel;
            model.Should().NotBeNull();

            var accountId = _testData.Get<long>("AccountId");
            var response = _testData.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var hashedAccountId = _testData.Get<string>("HashedAccountId");

            response.EnsureSuccessStatusCode();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("You cannot apply for this grant yet");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/cannot-apply");

            var requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath($"/accounts/{accountId}/legalentities")
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);
        }
         
        [Then(@"the employer is informed they cannot apply for the grant")]
        public async Task ThenTheEmployerIsInformedTheycannotApplyForTheGrant()
        {
            _testContext.ActionResult.LastViewResult.Should().NotBeNull();

            var accountId = _testData.Get<long>("AccountId");
            var accountLegalEntityId = _testData.Get<long>("AccountLegalEntityId");
            var response = _testData.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var hashedAccountId = _testData.Get<string>("HashedAccountId");

            response.EnsureSuccessStatusCode();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("You cannot apply for this grant");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/cannot-apply?hasTakenOnNewApprentices=True");

            var requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath($"/accounts/{accountId}/legalentities")
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);

            requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath("/apprenticeships")
                           .WithParam("accountid", accountId.ToString())
                           .WithParam("accountlegalentityid", accountLegalEntityId.ToString())
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);
        }

        [Then(@"the employer is asked if they have taken on qualifying apprenticeships")]
        public async Task ThenTheEmployerIsAskedIfTheyHavetakenOnQualifyingApprenticeships()
        {
            _testContext.ActionResult.LastViewResult.Should().NotBeNull();

            var response = _testData.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var accountId = _testData.Get<long>("AccountId");
            var accountLegalEntityId = _testData.Get<long>("AccountLegalEntityId");
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");
            
            response.EnsureSuccessStatusCode();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("Have you taken on new apprentices that joined your payroll after 1 August 2020?");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices");
            
            var requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath($"/accounts/{accountId}/legalentities")
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);

            requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath("/apprenticeships")
                           .WithParam("accountid", accountId.ToString())
                           .WithParam("accountlegalentityid", accountLegalEntityId.ToString())
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);
        }

        [Then(@"the employer is asked to select the legal entity the grant applies to")]
        public async Task ThenTheEmployerIsAskedToSelectTheLegalEntityTheGrantIsFor()
        {
            _testContext.ActionResult.LastViewResult.Should().NotBeNull();
            var model = _testContext.ActionResult.LastViewResult.Model as ChooseOrganisationViewModel;
            model.Should().NotBeNull();

            var response = _testData.Get<HttpResponseMessage>("ApplicationEligibilityResponse");            
            var accountId = _testData.Get<long>("AccountId");
            var hashedAccountId = _testData.Get<string>("HashedAccountId");

            response.EnsureSuccessStatusCode();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("Choose organisation");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/choose-organisation");

            var requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath($"/accounts/{accountId}/legalentities")
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);
        }
    }
}
