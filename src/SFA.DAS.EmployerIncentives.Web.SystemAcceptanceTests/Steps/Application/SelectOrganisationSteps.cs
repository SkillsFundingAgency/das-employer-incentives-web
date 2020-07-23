using AngleSharp.Html.Parser;
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
        public async Task ThenTheEmployerIsAskedIfTheyHavetakenOnQualifyingApprenticeships()
        {
            _testContext.ActionResult.LastViewResult.Should().NotBeNull();

            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            response.EnsureSuccessStatusCode();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("Have you taken on new apprentices that joined your payroll after 1 August 2020?");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices");            
        }

        [Then(@"the employer is informed that a legal entity needs to be selected")]
        public async Task ThenTheEmployerIsAskedToSelectTheLegalEntityTheGrantIsFor()
        {
            _testContext.ActionResult.LastViewResult.Should().NotBeNull();
            var model = _testContext.ActionResult.LastViewResult.Model as ChooseOrganisationViewModel;
            model.Should().NotBeNull();
            var error = _testContext.ActionResult.LastViewResult.ViewData.ModelState["OrganisationNotSelected"];
            error.Errors.Count().Should().Be(1);

            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");            
            var accountId = _testDataStore.Get<long>("AccountId");
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");

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
