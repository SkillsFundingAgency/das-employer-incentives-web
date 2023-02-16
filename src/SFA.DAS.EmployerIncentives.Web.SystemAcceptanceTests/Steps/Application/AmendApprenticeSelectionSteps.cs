using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Hub;
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
    [Scope(Feature = "AmendApprenticeSelection")]
    public class AmendApprenticeSelectionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly IAccountEncodingService _encodingService;
        private List<ApprenticeDto> _apprenticeshipData;
        private readonly TestData.Account.WithInitialApplicationForASingleEntity _data;
        private readonly ApplicationResponse _getApplicationResponse;
        
        private HttpResponseMessage _response;
        private LegalEntityDto _legalEntity;

        public AmendApprenticeSelectionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _encodingService = _testContext.EncodingService;
            _data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _getApplicationResponse = _data.GetApplicationResponseWithFirstTwoApprenticesSelected;
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _data.HashedAccountId);
        }

        [Given(@"there are eligible apprenticeships for the grant")]
        public void GivenThereAreEligibleApprenticeshipsForTheGrant()
        {
            _apprenticeshipData = _data.Apprentices;
            _legalEntity = _data.LegalEntities.First();

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", _data.AccountId.ToString())
                        .WithParam("accountlegalentityid", _data.AccountLegalEntityId.ToString())
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
        }

        [Given(@"a initial application has been created")]
        public void GivenAInitialApplicationHasBeenCreated()
        {
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonSerializer.Serialize(_getApplicationResponse, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                  Request
                      .Create()
                      .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}/accountlegalentity")
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(_data.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/accounts/{_data.AccountId}/legalentities")
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithStatusCode(HttpStatusCode.OK)
                  .WithBody(JsonSerializer.Serialize(_data.LegalEntities, TestHelper.DefaultSerialiserSettings)));
        }

        [Given(@"a initial application has been created and submitted")]
        public void GivenAInitialApplicationHasBeenCreatedAndSubmitted()
        {
            _getApplicationResponse.Application.SubmittedByEmail = "SubmittedBy@test.co.uk";
            GivenAInitialApplicationHasBeenCreated();
        }

        [Given(@"a initial application has been created and it includes an apprentice who is no longer eligible")]
        public void GivenAInitialApplicationHasBeenCreatedAndItIncludesAnApprenticeWhoIsNoLongerEligible()
        {
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonSerializer.Serialize(_data.GetApplicationResponseWithFirstTwoApprenticesSelectedAndAnAdditionalApprentice, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                  .Given(
                      Request
                          .Create()
                          .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}/accountlegalentity")
                          .UsingGet()
                  )
                  .RespondWith(
                      Response.Create()
                          .WithStatusCode(HttpStatusCode.OK)
                          .WithHeader("Content-Type", "application/json")
                          .WithBody(_data.AccountLegalEntityId.ToString()));
        }

        [When(@"the employer updates application with no apprentices selected")]
        public async Task WhenTheEmployerUpdatesApplicationWithNoApprenticesSelected()
        {
            var hashedAccountId = _encodingService.Encode(_data.AccountId);
            var url = $"{hashedAccountId}/apply/select-apprentices/{_data.ApplicationId}";
            var form = new SelectApprenticeshipsRequest();

            _response = await _testContext.WebsiteClient.PostAsJsonAsync(url, form);
        }

        [When(@"the employer updates application with apprentices selected")]
        public async Task WhenTheEmployerUpdatesApplicationWithApprenticesSelected()
        {
            var hashedAccountId = _encodingService.Encode(_data.AccountId);
            var url = $"{hashedAccountId}/apply/select-apprentices/{_data.ApplicationId}";
            var form = new KeyValuePair<string, string>("SelectedApprenticeships", _encodingService.Encode(1));
            SetupEndpointForUpdateApplication();

            _response = await _testContext.WebsiteClient.PostFormAsync(url, form);
        }

        [When(@"the employer returns to the select apprentices page")]
        public async Task WhenTheEmployerReturnsToTheSelectApprenticesPage()
        {
            var hashedAccountId = _encodingService.Encode(_data.AccountId);

            var url = $"{hashedAccountId}/apply/select-apprentices/{_data.ApplicationId}";

            _response = await _testContext.WebsiteClient.GetAsync(url);
            _response.EnsureSuccessStatusCode();
        }

        [Then(@"the user is directed to the employment start dates page")]
        public void ThenTheUserIsDirectedToTheEmploymentStartDatesPage()
        {
            var hashedAccountId = _encodingService.Encode(_data.AccountId);
            _response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            _response.Should().HaveBackLink($"/{hashedAccountId}/apply/select-apprentices/{model.ApplicationId}");
            model.Should().HaveTitle($"When did they join {_legalEntity.LegalEntityName}?");
        }

        [Then(@"the user is directed to the hub page")]
        public void ThenTheUserIsDirectedToTheHubPage()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_data.HashedAccountId}/{_data.HashedAccountLegalEntityId}/hire-new-apprentice-payment");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HubPageViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Hire a new apprentice payment");
        }

        [Then(@"the employer will receive an error")]
        public void ThenTheEmployerWillReceiveAnError()
        {
            var hashedAccountId = _encodingService.Encode(_data.AccountId);
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            _response.Should().HaveTitle(model.Title);
            _response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/select-apprentices/{_data.ApplicationId}");
            model.Should().HaveTitle("Which apprentices do you want to apply for?");
            viewResult.Should().ContainError(model.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
        }

        [Then(@"the employer can see the previous apprentices checked")]
        public void ThenTheEmployerCanSeeThePreviousApprenticesChecked()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();

            model.Apprenticeships.Count().Should().Be(3);

            model.Apprenticeships.First(x => x.Id == _encodingService.Encode(1)).Selected.Should().BeTrue();
            model.Apprenticeships.First(x => x.Id == _encodingService.Encode(2)).Selected.Should().BeTrue();
            model.Apprenticeships.First(x => x.Id == _encodingService.Encode(3)).Selected.Should().BeFalse();

            model.AccountId.Should().Be(_data.HashedAccountId);
            model.AccountLegalEntityId.Should().Be(_data.HashedAccountLegalEntityId);

            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/{_data.HashedAccountLegalEntityId}/eligible-apprentices");

        }

        [Then(@"the additional apprentice is not on the list")]
        public void ThenTheAdditionalApprenticeIsNotOnTheList()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;

            model.Apprenticeships.FirstOrDefault(x => x.Id == _encodingService.Encode(99)).Should().BeNull();
        }

        private void SetupEndpointForUpdateApplication()
        {
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications")
                        .UsingPut()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK));
        }
    }
}
