using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System.Collections.Generic;
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
        private readonly TestDataStore _testDataStore;
        private readonly IHashingService _hashingService;

        public EligibilitySteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
            _hashingService = _testContext.HashingService;
        }

        [Given(@"an employer applying for a grant has no apprenticeships matching the eligibility requirement")]
        public void GivenAnEmployerApplyingHasNoApprenticesMatchingTheEligibilityCriteria()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithNoEligibleApprenticeships();

            var accountId = _testDataStore.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testDataStore.Add("HashedAccountId", _hashingService.HashValue(accountId));
            var accountLegalEntityId = _testDataStore.GetOrCreate("AccountLegalEntityId", onCreate: () => testdata.AccountLegalEntityId);
            _testDataStore.Add("HashedAccountLegalEntityId", _hashingService.HashValue(accountLegalEntityId));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/accounts/{accountId}/legalentities")
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings))
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

            var accountId = _testDataStore.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testDataStore.Add("HashedAccountId", _hashingService.HashValue(accountId));
            var accountLegalEntityId = _testDataStore.GetOrCreate("AccountLegalEntityId", onCreate: () => testdata.AccountLegalEntityId);
            _testDataStore.Add("HashedAccountLegalEntityId", _hashingService.HashValue(accountLegalEntityId));

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
                  .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

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
                  .WithBody(JsonConvert.SerializeObject(testdata.Apprentices, TestHelper.DefaultSerialiserSettings))
                  .WithStatusCode(HttpStatusCode.OK));
        }

        [Given(@"an employer applying for a grant has no legal entities")]
        public void GivenAnEmployerApplyingHasNoLegalentities()
        {
            var testdata = new TestData.Account.WithNoLegalEntites();

            var accountId = _testDataStore.GetOrCreate("AccountId", onCreate: () => testdata.AccountId);
            _testDataStore.Add("HashedAccountId", _hashingService.HashValue(accountId));

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
                  .WithBody(JsonConvert.SerializeObject(legalEntities, TestHelper.DefaultSerialiserSettings)));
        }

        [When(@"the employer tries to make a grant application")]
        public async Task WhenTheEmployerMakesAGrantApplication()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get, 
                $"{_testDataStore.Get<string>("HashedAccountId")}/apply")
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("hasTakenOnNewApprentices", "true")
                    })
                };            

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }

        [Then(@"the employer is informed they cannot apply for the grant yet")]
        public void ThenTheEmployerIsInformedTheycannotApplyForTheGrantYet()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as CannotApplyViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("You cannot apply for this grant yet");
            model.AccountId.Should().Be(hashedAccountId);
            model.CommitmentsUrl.Should().Be(_testContext.WebConfigurationOptions.CommitmentsBaseUrl);

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/cannot-apply");
        }
         
        [Then(@"the employer is informed they cannot apply for the grant")]
        public void ThenTheEmployerIsInformedTheycannotApplyForTheGrant()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as TakenOnCannotApplyViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("You cannot apply for this grant");
            model.AccountId.Should().Be(hashedAccountId);
            model.CommitmentsUrl.Should().Be(_testContext.WebConfigurationOptions.CommitmentsBaseUrl);
                        
            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/cannot-apply?hasTakenOnNewApprentices=True");
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

        [Then(@"the employer is asked to select the legal entity the grant applies to")]
        public void ThenTheEmployerIsAskedToSelectTheLegalEntityTheGrantIsFor()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;
            
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ChooseOrganisationViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(hashedAccountId);

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/choose-organisation");
        }
    }
}
