using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "PreApplication")]
    public class PreApplicationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestData.Account.WithInitialApplicationForASingleEntity _testdata;
        private HttpResponseMessage _response;
        private string _currentPage;

        public PreApplicationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testdata = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testdata.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountId", _testdata.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testdata.HashedAccountLegalEntityId);
        }

        [Given(@"the employer is on the hub page")]
        public async Task GivenTheEmployerIsOnTheHubPage()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}/hire-new-apprentice-payment");

            _response = await _testContext.WebsiteClient.SendAsync(request);
            _response.EnsureSuccessStatusCode();

            _currentPage = "hub";

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return _response;
            });
        }

        [Given(@"the employer is on the before you start page")]
        public async Task GivenTheEmployerIsOnTheBeforeYouStartPage()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}/before-you-start");

            _response = await _testContext.WebsiteClient.SendAsync(request);
            _response.EnsureSuccessStatusCode();

            _currentPage = "beforeStart";

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return _response;
            });
        }

        [Given(@"the employer is on the application information page")]
        public async Task GivenTheEmployerIsOnTheApplicationInformationPage()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}");

            _response = await _testContext.WebsiteClient.SendAsync(request);
            _response.EnsureSuccessStatusCode();

            _currentPage = "applyInformation";

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return _response;
            });
        }       

        [When(@"the employer selects the Hire a new apprentice payment link")]
        public async Task WhenTheEmployerSelectstheHireANewApprenticePaymentLink()
        {
            var url = $"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}/before-you-start";

            _response.Should().HaveLink("[data-linktype='hub-before-apply']", url);

            _response = await _testContext.WebsiteClient.GetAsync(url);
            _response.EnsureSuccessStatusCode();
        }

        [When(@"the employer selects the Start now button")]
        public async Task WhenTheEmployerSelectsTheStartNowButton()
        {
            string url = string.Empty;
            string linkType = string.Empty;

            switch (_currentPage)
            {
                case "beforeStart":
                    url = $"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}";
                    linkType = "before-apply-start";
                    break;
                case "applyInformation":
                    url = $"/{_testdata.HashedAccountId}/apply/{_testdata.HashedAccountLegalEntityId}/eligible-apprentices";
                    linkType = "apply-qualification";
                    break;
            }            

            _response.Should().HaveLink($"[data-linktype='{linkType}']", url);

            _response = await _testContext.WebsiteClient.GetAsync(url);
            _response.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is on the before you start page")]
        public void ThenTheUserIsOnTheBeforeYouStartPage()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as BeforeYouStartViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Before you start");
            _response.Should().HaveBackLink($"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}/hire-new-apprentice-payment");
            _response.Should().HavePathAndQuery($"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}/before-you-start");
            _response.Should().HaveLink("[data-linktype='before-apply-start']", $"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}");
        }

        [Then(@"the employer is on the application information page")]
        public void ThenTheUserIsOnTheApplicationInformationPage()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HomeViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Apply for the hire a new apprentice payment");
            _response.Should().HaveBackLink($"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}/before-you-start");
            _response.Should().HavePathAndQuery($"/{_testdata.HashedAccountId}/{_testdata.HashedAccountLegalEntityId}");
            _response.Should().HaveLink("[data-linktype='apply-qualification']", $"/{_testdata.HashedAccountId}/apply/{_testdata.HashedAccountLegalEntityId}/eligible-apprentices");
        }

        [Then(@"the employer is on the eligible apprentices page")]
        public void ThenTheUserIsOnTheEligibleApprenticesPage()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as QualificationQuestionViewModel;
            model.Should().NotBeNull();
        }
    }
}
