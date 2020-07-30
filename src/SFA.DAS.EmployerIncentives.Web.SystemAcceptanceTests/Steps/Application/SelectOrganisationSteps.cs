using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
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
    [Scope(Feature = "SelectOrganisation")]
    public class SelectOrganisationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;
        private readonly IHashingService _hashingService;
        public SelectOrganisationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
            _hashingService = _testContext.HashingService;
        }

        [Given(@"an employer applying for a grant has multiple legal entities")]
        public void GivenAnEmployerApplyingHasMultipleLegalentities()
        {
            var testdata = new TestData.Account.WithMultipleLegalEntities();

            var accountId = _testDataStore.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testDataStore.Add("HashedAccountId", _hashingService.HashValue(accountId));

            var legalEntities = _testDataStore.GetOrCreate("Legalentities", onCreate: () => testdata.LegalEntities);

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

        [When(@"the employer selects the legal entity the application is for")]
        public async Task WhenTheEmployerSelectsTheLegalEntityTheApplicationIsFor()
        {
            var testdata = new TestData.Account.WithMultipleLegalEntities();
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId2);

            var request = new HttpRequestMessage(
                HttpMethod.Post, 
                $"{testdata.HashedAccountId}/apply/choose-organisation")
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Selected", testdata.HashedAccountLegalEntityId2)
                    })
                };

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }

        [When(@"the employer does not select the legal entity the application is for")]
        public async Task WhenTheEmployerDoesNotSelectTheLegalEntityTheApplicationIsFor()
        {
            var testdata = new TestData.Account.WithMultipleLegalEntities();

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{testdata.HashedAccountId}/apply/choose-organisation")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                    })
            };

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }

        [Then(@"the employer is asked if they have taken on qualifying apprenticeships")]
        public void ThenTheEmployerIsAskedIfTheyHavetakenOnQualifyingApprenticeships()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as QualificationQuestionViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Have you taken on new apprentices that joined your payroll after 1 August 2020?");
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountLegalEntityId.Should().Be(hashedAccountLegalEntityId);
            model.HasTakenOnNewApprentices.Should().BeNull();

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices");
        }

        [Then(@"the employer is informed that a legal entity needs to be selected")]
        public void ThenTheEmployerIsAskedToSelectTheLegalEntityTheGrantIsFor()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ChooseOrganisationViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Choose organisation");
            model.AccountId.Should().Be(hashedAccountId);
            viewResult.Should().ContainError(model.Organisations.First().AccountLegalEntityId, model.OrganisationNotSelectedMessage);

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/choose-organisation");
        }
    }
}
