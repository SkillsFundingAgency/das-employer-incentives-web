using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
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

        [Given(@"an employer applying for a grant has multiple legal entities with eligible apprenticeships")]
        public void GivenAnEmployerApplyingHasMultipleLegalentitiesWithEligibleApprenticeships()
        {
            var testdata = new TestData.Account.WithMultipleLegalEntitiesWithEligibleApprenticeships();

            var accountId = _testDataStore.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testDataStore.Add("HashedAccountId", _hashingService.HashValue(accountId));
            _testDataStore.Add("HashedAccountLegalEntityId", _hashingService.HashValue(testdata.LegalEntities.First().AccountLegalEntityId));

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
                  .WithBody(JsonConvert.SerializeObject(legalEntities, TestHelper.DefaultSerialiserSettings)));

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
                  .WithBody(JsonConvert.SerializeObject(testdata.Apprentices, TestHelper.DefaultSerialiserSettings))
                  .WithStatusCode(HttpStatusCode.OK));
        }
        
        [When(@"the employer selects the legal entity the application is for")]
        public async Task WhenTheEmployerSelectsTheLegalEntityTheApplicationIsFor()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            var request = new HttpRequestMessage(
                HttpMethod.Post, 
                $"{hashedAccountId}/apply/choose-organisation")
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Selected", hashedAccountLegalEntityId)
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
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{hashedAccountId}/apply/choose-organisation")
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

        [Then(@"the employer is asked to select the apprenticeship")]
        public void ThenTheEmployerIsAskedWoSelectTheApprenticeship()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Select the apprentices you want to apply for");
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountLegalEntityId.Should().Be(hashedAccountLegalEntityId);

            response.Should().HaveTitle(model.Title);
            response.Should().HaveBackLink($"/{hashedAccountId}/apply/taken-on-new-apprentices");
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/select-new-apprentices");
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
