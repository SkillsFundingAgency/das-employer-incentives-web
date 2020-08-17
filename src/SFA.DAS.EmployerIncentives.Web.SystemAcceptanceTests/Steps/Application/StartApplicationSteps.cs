using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
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
    [Scope(Feature = "StartApplication")]
    public class StartApplicationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;

        public StartApplicationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
        }

        [Given(@"an employer with a single organisation wants to apply for a grant")]
        public void GivenAnEmployerWithASingleOrganisationWantsToApplyForAGrant()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);

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
               .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

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

        [Given(@"an employer with a multiple organisations wants to apply for a grant")]
        public void GivenAnEmployerWithAMultipleOrganisationsWantsToApplyForAGrant()
        {
            var testdata = new TestData.Account.WithMultipleLegalEntitiesWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId1);

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
               .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

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

        [Given(@"an employer without any organisations wants to apply for a grant")]
        public void GivenAnEmployerWithoutAnyOrganisationsWantsToApplyForAGrant()
        {
            var testdata = new TestData.Account.WithNoLegalEntites();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
           .Given(
                   Request
                   .Create()
                   .WithPath($"/accounts/{testdata.AccountId}/legalentities")
                   .UsingGet()
                   )
               .RespondWith(
             Response.Create()
               .WithStatusCode(HttpStatusCode.NotFound));
        }

        [When(@"the employer applies for the grant")]
        public async Task WhenTheEmployerAppliesForTheGrant()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{hashedAccountId}/apply");

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
        }

        [Then(@"the employer is informed that they need to specify whether or not they have eligible apprenticeships")]
        public void ThenTheEmployerIsInformedThatTheyNeedToSpecifyWhetherOrNotTheyHaveEligibleApprenticeships()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as QualificationQuestionViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Have you taken on new apprentices who started their contract of employment between 1 August 2020 and 31 January 2021?");
            model.AccountId.Should().Be(hashedAccountId);

            response.Should().HaveTitle(model.Title);
            response.Should().HaveBackLink($"/{hashedAccountId}");
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices");
        }

        [Then(@"the employer is asked to choose the organisation")]
        public void ThenTheEmployerIsAskedToChooseTheOrganisation()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ChooseOrganisationViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Choose organisation");
            model.AccountId.Should().Be(hashedAccountId);

            response.Should().HaveTitle(model.Title);
            response.Should().HaveBackLink($"/{hashedAccountId}");
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/choose-organisation");
        }

        [Then(@"the employer is informed that they cannot apply")]
        public void ThenTheEmployerIsInformedThatTheyCannotApply()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as CannotApplyViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("You can only apply for apprentices who started their contract of employment between 1 August 2020 and 31 January 2021");
            model.AccountId.Should().Be(hashedAccountId);
            model.AddApprenticesUrl.Should().Be($"{_testContext.WebConfigurationOptions.CommitmentsBaseUrl}/commitments/accounts/{hashedAccountId}/apprentices/inform");

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/cannot-apply");
        }
    }
}
