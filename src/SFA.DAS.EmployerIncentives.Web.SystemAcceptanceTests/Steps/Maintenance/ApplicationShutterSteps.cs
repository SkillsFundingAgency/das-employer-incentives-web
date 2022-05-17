using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Hooks;
using SFA.DAS.EmployerIncentives.Web.ViewModels.System;
using TechTalk.SpecFlow;

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
    }
}
