using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Maintenance
{
    [Binding]
    [Scope(Feature = "ApplicationShutter")]
    public class ApplicationShutterSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;
        private AuthorizationHandlerContext _authContext;

        public ApplicationShutterSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;

            var hook = _testContext.Hooks.SingleOrDefault(h => h is Hook<AuthorizationHandlerContext>) as Hook<AuthorizationHandlerContext>;
            hook.OnProcessed = (c) => {
                if (_authContext == null)
                {
                    _authContext = c;
                }
            };
        }

        [Given(@"the application is configured to prevent applications")]
        public void GivenTheApplicationIsConfiguredToPreventApplications()
        {
            // no implementation - uses the applyApplicationShutterPage tag
        }

        [Given(@"the application is configured to allow applications")]
        public void GivenTheApplicationIsConfiguredToNotPreventApplications()
        {
            // no implementation - uses the applyApplicationShutterPage tag
        }

        [When(@"the employer submits an application for the new apprenticeship payment")]
        public async Task WhenTheEmployerSubmitsAnApplicationForTheApprenticeshipPayment()
        {
            var testdata = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);

            var url = $"{testdata.HashedAccountId}/apply/declaration/{testdata.ApplicationId}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
        }

        [When(@"the employer access the (.*) page")]
        public async Task WhenTheEmployerAccessesTheHomePage(string urlString)
        {
            _testDataStore.Add("HashedAccountId", "VBKBLD");
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, "VBKBLD");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{urlString}");

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
        }

        [When(@"the employer is on the hub page")]
        public async Task WhenTheEmployerIsOnTheHubPage()
        {
            var testdata = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);
            _testDataStore.Add("LegalEntityName", testdata.LegalEntity.LegalEntityName);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);

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

            var request = new HttpRequestMessage(HttpMethod.Get, $"{testdata.HashedAccountId}/{testdata.HashedAccountLegalEntityId}/hire-new-apprentice-payment");

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
        }

        [Then(@"the employer is shown the application shutter page")]
        public void ThenTheEmployerIsShownTheApplicationShutterPage()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationsClosedModel;
            model.Should().NotBeNull();

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/applications-closed");
            response.Should().HaveLink("[data-linktype='applications-closed-return']", $"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{hashedAccountId}/teams");
        }

        [Then(@"the employer is not shown the application shutter page")]
        public void ThenTheEmployerIsNotShownTheApplicationShutterPage()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;

            if(viewResult != null)
            {
                var model = viewResult.Model as ApplicationsClosedModel;
                model.Should().BeNull();
            }
        }

        [Then(@"the employer is not shown the apply link")]
        public void ThenTheEmployerIsNotShownTheApplyLink()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var url = $"/{hashedAccountId}/{hashedAccountLegalEntityId}/before-you-start";

            response.Should().NotHaveLink("[data-linktype='hub-before-apply']", url);
        }

        [Then(@"the employer is shown a link to the guidance page")]
        public void ThenTheEmployerIsShownTheGuidanceLink()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var url = "https://help.apprenticeships.education.gov.uk/hc/en-gb/articles/4403316291090-Incentive-payment-for-hiring-a-new-apprentice-view-your-application";

            response.Should().HaveLink("[data-linktype='incentive-payment-guidance']", url);
        }

        [Then(@"the heading text indicates that they can apply for incentive payments")]
        public void ThenTheHeadingTextContainsApply()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var legalEntityName = _testDataStore.Get<string>("LegalEntityName");
            response.Should().HaveInnerHtml("[data-paragraphtype='hub-heading']", $"Apply for the payment and view {legalEntityName}'s applications.");
        }


        [Then(@"the heading text does not indicate that they can apply for incentive payments")]
        public void ThenTheHeadingTextDoesNotContainApply()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var legalEntityName = _testDataStore.Get<string>("LegalEntityName");
            response.Should().HaveInnerHtml("[data-paragraphtype='hub-heading']", $"View {legalEntityName}'s applications or change their organisation and finance details.");
        }
    }
}
