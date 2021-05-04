using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "TermsAgreed")]
    public class TermsAgreedSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;

        public TermsAgreedSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
        }

        [Given(@"an employer who has signed the incentive terms applying for a grant")]
        public void GivenAnEmployerWhoHasSignedTheIncentiveTermsIsApplyingForAGrant()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
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
                        .WithPath($"/accounts/{testdata.AccountId}/legalentities/{testdata.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities.First(), TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
             .Given(
                     Request
                     .Create()
                     .WithPath($"/apprenticeships")
                     .WithParam("accountid", testdata.AccountId.ToString())
                     .WithParam("accountlegalentityid", testdata.LegalEntities.First().AccountLegalEntityId.ToString())
                     .WithParam("pageNumber", "1")
                     .UsingGet()
                     )
                 .RespondWith(
             Response.Create()
                 .WithBody(JsonConvert.SerializeObject(testdata.Apprentices, TestHelper.DefaultSerialiserSettings))
                 .WithStatusCode(HttpStatusCode.OK));
        }

        [Given(@"an employer who has not signed the incentive terms applying for a grant")]
        public void GivenAnEmployerWhoHasNotSignedTheIncentiveTermsIsApplyingForAGrant()
        {
            var testdata = new TestData.Account.WithoutASignedAgreement();
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
                        .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

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
                        .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities.First(), TestHelper.DefaultSerialiserSettings)));

        }

        [When(@"the employer completes eligibility")]
        public async Task WhenTheEmployerCompletesEligibility()
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

        [Then(@"the employer is asked to select the apprenticeship")]
        public void ThenTheEmployerIsAskedToSelectTheApprenticeship()
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

        [Then(@"the employer is asked to sign a new legal agreement")]
        public void ThenTheEmployerIsAskedToSignANewLegalAgreement()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var legalEntity = _testDataStore.Get<LegalEntityDto>("LegalEntity");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ValidateTermsSignedViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle($"{legalEntity.LegalEntityName} needs to accept the employer agreement");
            model.AccountId.Should().Be(hashedAccountId);

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/validate-terms-signed");
        }
    }
}
