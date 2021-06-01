using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;
using SFA.DAS.EmployerIncentives.Web.ViewModels.System;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.System
{
    [Binding]
    [Scope(Feature = "ApplicationShutter")]
    public class ApplicationShutterSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;

        public ApplicationShutterSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
        }
        [Given(@"the application is configured to prevent applications")]
        public void GivenTheApplicationIsConfiguredToPreventApplications()
        {
            // no implementation - uses the applyApplicationShutterPage tag
        }

        [When(@"the employer applies for the hire a new apprenticeship payment")]
        public async Task WhenTheEmployerAppliesForTheApprenticeshipPayment()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{testdata.HashedAccountId}/{testdata.HashedAccountLegalEntityId}");

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
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

        [Then(@"the employer is shown the application shutter page")]
        public void ThenTheEmployerIsShownTheApplicationShutterPage()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SystemUpdateModel;
            model.Should().NotBeNull();

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/system-update");
            response.Should().HaveLink("[data-linktype='system-update-return']", $"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{hashedAccountId}/teams");
        }

        [Then(@"the employer is shown the start page")]
        public void ThenTheEmployerIsShownTheStartPage()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HomeViewModel;
            model.Should().NotBeNull();
            response.Should().HaveTitle(model.Title);
        }

        [Then(@"the employer is asked to enter bank details")]
        public void ThenTheEmployerIsAskedToEnterBankDetails()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as BankDetailsConfirmationViewModel;
            model.Should().NotBeNull();
            response.Should().HaveTitle(model.Title);
        }
    }
}
