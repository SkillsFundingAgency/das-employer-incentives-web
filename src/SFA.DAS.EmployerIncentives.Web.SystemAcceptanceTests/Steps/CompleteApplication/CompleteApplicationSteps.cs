using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.CompleteApplication
{
    [Binding]
    [Scope(Feature = "CompleteApplication")]
    public class CompleteApplicationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestData.Account.WithInitialApplicationAndBankingDetails _testdata;
        private readonly Fixture _fixture;

        public CompleteApplicationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testdata = new TestData.Account.WithInitialApplicationAndBankingDetails();
            _fixture = new Fixture();
        }
   
        [Given(@"given the employer has all the information required to process their bank details")]
        public void GivenTheEmployerHasAllTheInformationRequiredToProcessTheirBankDetails()
        {            
            _testContext.TestDataStore.Add("HashedAccountId", _testdata.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testdata.HashedAccountLegalEntityId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testdata.HashedAccountId);
            
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testdata.AccountId}/applications/{_testdata.ApplicationId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(_testdata.ApplicationResponse)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testdata.AccountId}/applications/{_testdata.ApplicationId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(_testdata.ApplicationResponse)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testdata.AccountId}/applications/{_testdata.ApplicationId}/accountlegalentity")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_testdata.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testdata.AccountId}/applications/{_testdata.ApplicationId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonConvert.SerializeObject(_testdata.ApplicationResponse, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            var getBankingDetailsUrl = "/" + OuterApiRoutes.Application.GetBankingDetailsUrl(_testdata.AccountId, _testdata.ApplicationId, _testdata.HashedAccountId).Split("?").First();
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(getBankingDetailsUrl)
                        .WithParam("hashedAccountId", _testdata.HashedAccountId)
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_testdata.BankingDetails, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
            .Given(
                Request
                    .Create()
                    .WithPath($"/accounts/{_testdata.AccountId}/legalentities/{_testdata.AccountLegalEntityId}")
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonConvert.SerializeObject(_testdata.LegalEntity)));

           _testContext.EmployerIncentivesApi.MockServer
           .Given(
                   Request
                   .Create()
                   .WithPath($"/accounts/{_testdata.AccountId}/legalentities")
                   .UsingGet()
                   )
               .RespondWith(
             Response.Create()
               .WithStatusCode(HttpStatusCode.OK)
               .WithBody(JsonConvert.SerializeObject(_testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testdata.AccountId}/legalentities/{_testdata.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(_testdata.LegalEntity, TestHelper.DefaultSerialiserSettings)));
        }

        [When(@"the employer provides their bank details")]
        public async Task WhenTheEmployerProvidesTheirBankDetails()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_testdata.HashedAccountId}/bank-details/{_testdata.ApplicationId}/enter-bank-details");

            var continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            continueNavigationResponse.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);

            continueNavigationResponse.RequestMessage.RequestUri.AbsolutePath.Should().Be("/service/provide-organisation-information");
            var queryParams = continueNavigationResponse.RequestMessage.RequestUri.ParseQueryString();
            queryParams.Should().Contain("journey");
            queryParams.Should().Contain("return");
            queryParams["journey"].Should().Be("new");
            var returnUri = new Uri(HttpUtility.UrlDecode(queryParams["return"]));
            returnUri.PathAndQuery.Should().Be($"/{_testdata.HashedAccountId}/application-complete/{_testdata.ApplicationId}");

            request = new HttpRequestMessage(
                HttpMethod.Get,
                returnUri.PathAndQuery);

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"the employer completes their application journey")]
        public void ThenTheEmployerCompletesTheirApplicationJourney()
        {
            var hashedAccountId = _testContext.TestDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testContext.TestDataStore.Get<string>("HashedAccountLegalEntityId");
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmationViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountLegalEntityId.Should().Be(hashedAccountLegalEntityId);
            model.OrganisationName.Should().Be(_testdata.LegalEntity.LegalEntityName);
        }
    }
}
