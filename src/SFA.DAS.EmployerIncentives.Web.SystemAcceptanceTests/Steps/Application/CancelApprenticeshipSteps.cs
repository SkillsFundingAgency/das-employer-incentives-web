using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Cancel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "CancelApprenticeship")]
    public class CancelApprenticeshipSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testData;
        private readonly IAccountEncodingService _encodingService;
        private HttpResponseMessage _continueNavigationResponse;
        private List<ApprenticeshipIncentiveModel> _apprenticeshipData;
        private LegalEntityDto _legalEntity;
        private TestData.Account.WithPreviousApprenticeshipIncentiveForFirstLegalEntity _data;

        public CancelApprenticeshipSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = _testContext.TestDataStore;
            _encodingService = _testContext.EncodingService;
            _data = new TestData.Account.WithPreviousApprenticeshipIncentiveForFirstLegalEntity();
        }

        [Given(@"an employer applying for a grant has existing applied for apprenticeship incentives")]
        public void GivenAnEmployerApplyingForAGrantHasExistingAppliedForApprenticeshipIncentives()
        {            
            _apprenticeshipData = _data.ApprenticeshipIncentives;
            _legalEntity = _data.LegalEntities.First();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => _data.AccountId);
            _testData.Add("HashedAccountId", _encodingService.Encode(accountId));
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _encodingService.Encode(accountId));
            var accountLegalEntityId = _testData.GetOrCreate("AccountLegalEntityId", onCreate: () => _legalEntity.AccountLegalEntityId);
            _testData.Add("HashedAccountLegalEntityId", _encodingService.Encode(accountLegalEntityId));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{accountId}/legalentities/{_legalEntity.AccountLegalEntityId}/apprenticeshipIncentives")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonSerializer.Serialize(_apprenticeshipData, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/legalentities/{_legalEntity.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonSerializer.Serialize(_legalEntity))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/withdrawals")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Accepted));
        }

        [Given(@"an employer applying for a grant has no existing applied for apprenticeship incentives")]
        public void GivenAnEmployerApplyingForAGrantHasNoExistingAppliedForApprenticeshipIncentives()
        {
            _apprenticeshipData = new List<ApprenticeshipIncentiveModel>();
            _legalEntity = _data.LegalEntities.First();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => _data.AccountId);
            _testData.Add("HashedAccountId", _encodingService.Encode(accountId));
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _encodingService.Encode(accountId));
            var accountLegalEntityId = _testData.GetOrCreate("AccountLegalEntityId", onCreate: () => _legalEntity.AccountLegalEntityId);
            _testData.Add("HashedAccountLegalEntityId", _encodingService.Encode(accountLegalEntityId));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{accountId}/legalentities/{_legalEntity.AccountLegalEntityId}/apprenticeshipIncentives")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonSerializer.Serialize(_apprenticeshipData, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/legalentities/{_legalEntity.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonSerializer.Serialize(_legalEntity))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
             .Given(
                 Request
                     .Create()
                     .WithPath($"/accounts/{_data.AccountId}/legalentity/{_legalEntity.AccountLegalEntityId}/applications")
                     .UsingGet()
             )
             .RespondWith(
                 Response.Create()
                     .WithStatusCode(HttpStatusCode.OK)
                     .WithHeader("Content-Type", "application/json")
                     .WithBody(JsonSerializer.Serialize(_data.GetApplicationsResponse, TestHelper.DefaultSerialiserSettings)));
        }

        [Given(@"an employer has selected apprenticeship incentives to cancel")]
        public async Task GivenAnEmployerHasSelectedApprenticeshipIncentivesToCancel()
        {
            GivenAnEmployerApplyingForAGrantHasExistingAppliedForApprenticeshipIncentives();
            await WhenTheEmployerSelectsTheApprenticeshipsToCancel();
        }

        [When(@"the employer views the cancel apprenticeships page")]
        public async Task WhenTheEmployerViewsTheCancelApprenticeshipsPage()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/cancel/{hashedLegalEntityId}/cancel-application";

            _continueNavigationResponse = await _testContext.WebsiteClient.GetAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"the employer selects the apprenticeships to cancel")]
        public async Task WhenTheEmployerSelectsTheApprenticeshipsToCancel()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/cancel/{hashedLegalEntityId}/confirm-cancel-application";
            var form = new KeyValuePair<string, string>("SelectedApprenticeships", _data.ApprenticeshipIncentive4.Id);

            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url, form);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"the employer doesn't select any apprenticeships to cancel")]
        public async Task WhenTheEmployerDoesntSelectAnyApprenticeshipsToCancel()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/cancel/{hashedLegalEntityId}/confirm-cancel-application";

            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"the employer wants to change the selected apprenticeships")]
        public async Task WhenTheEmployerWantsToChangeSelectsTheApprenticeshipsToCancel()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"/{hashedAccountId}/cancel/{hashedLegalEntityId}/cancel-application";
            _continueNavigationResponse.Should().HaveLink("[data-linktype='cancel-change']", url);

            _continueNavigationResponse = await _testContext.WebsiteClient.GetAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"the employer submits the selected apprenticeships for cancellation")]
        public async Task WhenTheEmployerSubmitsTheSelectedApprenticeshipsForCancellation()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"/{hashedAccountId}/cancel/{hashedLegalEntityId}/application-cancelled";
            _continueNavigationResponse.Should().HaveForm(url);
            _continueNavigationResponse.Should().HaveButton("[data-buttontype='cancel-confirm']", "Confirm");

            var form = new KeyValuePair<string, string>("SelectedApprenticeships", _data.ApprenticeshipIncentive4.Id);
            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url, form);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked to select the apprenticeships to cancel")]
        public void ThenTheEmployerIsAskedToSelectTheApprenticeshipsToCancel()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            
            model.ApprenticeshipIncentives.Should().BeEquivalentTo(_data.ApprenticeshipIncentives);
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/cancel/{hashedLegalEntityId}/cancel-application");
            _continueNavigationResponse.Should().HaveBackLink($"/{hashedAccountId}/payments/{hashedLegalEntityId}/payment-applications");
            model.Should().HaveTitle($"Which apprentices do you want to cancel an application for?");
        }

        [Then(@"the employer is asked to confirm the selected apprenticeships")]
        public void ThenTheEmployerIsAskedToConfirmTheSelectedApprenticeships()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmApprenticeshipsViewModel;
            model.Should().NotBeNull();
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/cancel/{hashedLegalEntityId}/confirm-cancel-application");
            _continueNavigationResponse.Should().HaveBackLink($"/{hashedAccountId}/cancel/{hashedLegalEntityId}/cancel-application");
            model.Should().HaveTitle($"Confirm apprentices");
        }

        [Then(@"the employer is redirected to the view applications page")]
        public void ThenTheEmployerIsRedirectedToThePaymentsPage()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ViewApplicationsViewModel;
            model.Should().NotBeNull();
        }

        [Then(@"the employer is informed that they haven't selected any apprenticeships")]
        public void ThenTheEmployerIsInformedThatTheyHaventSelectedAnyApprenticeships()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            _continueNavigationResponse.Should().HaveTitle(model.Title);
            _continueNavigationResponse.Should().HavePathAndQuery($"/{hashedAccountId}/cancel/{hashedLegalEntityId}/confirm-cancel-application");
            model.Should().HaveTitle($"Which apprentices do you want to cancel an application for?");
            model.ApprenticeshipIncentives.Should().BeEquivalentTo(_data.ApprenticeshipIncentives);
            model.AccountId.Should().Be(hashedAccountId);
            viewResult.Should().ContainError(model.FirstCheckboxId, new SelectApprenticeshipsViewModel().SelectApprenticeshipsMessage);
        }

        [Then(@"the employer is shown the cancel apprenticeships page with the selected apprenticeships checked")]
        public void ThenTheEmployerIsShownTheCancelApprenticeshipsPageWithTheSelectedApprenticeshipsChecked()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();

            model.ApprenticeshipIncentives.Single(a => a.Selected).Id.Should().Be(_data.ApprenticeshipIncentive4.Id);
            model.ApprenticeshipIncentives.Count(a => !a.Selected).Should().Be(_data.ApprenticeshipIncentives.Count - 1);
        }

        [Then(@"the employer is shown the application cancelled page")]
        public void ThenTheEmployerIsShownTheApplicationCancelledPage()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as CancelledApprenticeshipsViewModel;
            model.Should().NotBeNull();

            _continueNavigationResponse.Should().HaveLink("[data-linktype='cancel-view-applications']", $"/{hashedAccountId}/payments/{hashedLegalEntityId}/payment-applications");
            _continueNavigationResponse.Should().HaveLink("[data-linktype='cancel-survey']", @"https://www.smartsurvey.co.uk/s/HANAIP");
        }
    }
}
