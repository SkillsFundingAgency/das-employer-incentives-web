using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    public class AmendApprenticeSelectionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly IHashingService _hashingService;
        private EligibleApprenticesDto _apprenticeshipData;
        private TestData.Account.WithInitialApplicationForASingleEntity _data;
        private HttpResponseMessage _response;

        public AmendApprenticeSelectionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _hashingService = _testContext.HashingService;
            _data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _data.HashedAccountId);
        }

        [Given(@"there are eligible apprenticeships for the grant")]
        public void GivenThereAreEligibleApprenticeshipsForTheGrant()
        {
            _apprenticeshipData = _data.Apprentices;

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
                        .WithBody(JsonConvert.SerializeObject(_apprenticeshipData, TestHelper.DefaultSerialiserSettings))
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
                        .WithBody(JsonConvert.SerializeObject(_data.GetApplicationResponseWithFirstTwoApprenticesSelected, TestHelper.DefaultSerialiserSettings)));

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
                        .WithBody(JsonConvert.SerializeObject(_data.GetApplicationResponseWithFirstTwoApprenticesSelectedAndAnAdditionalApprentice, TestHelper.DefaultSerialiserSettings)));

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
                        .WithBody(JsonConvert.SerializeObject(_data.EmptyApplicationResponse, TestHelper.DefaultSerialiserSettings)));

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

            var hashedAccountId = _hashingService.HashValue(_data.AccountId);
            var url = $"{hashedAccountId}/apply/complete-apprentices/{_data.ApplicationId}";
            var form = new SelectApprenticeshipsRequest 
            {
                AccountId = _data.HashedAccountId, 
                AccountLegalEntityId = _data.HashedAccountLegalEntityId, 
                ApplicationId = _data.ApplicationId, 
                SelectedApprenticeships = null, 
                CurrentPage = 1
            };
            SetupEndpointForUpdateApplication();

            _response = await _testContext.WebsiteClient.PostAsJsonAsync(url, form);
        }

        [When(@"the employer updates application with apprentices selected")]
        public async Task WhenTheEmployerUpdatesApplicationWithApprenticesSelected()
        {
            var hashedAccountId = _hashingService.HashValue(_data.AccountId);
            var url = $"{hashedAccountId}/apply/complete-apprentices/{_data.ApplicationId}";
            var form = new KeyValuePair<string, string>("SelectedApprenticeships", _hashingService.HashValue(1));
            SetupEndpointForUpdateApplication();

            _response = await _testContext.WebsiteClient.PostFormAsync(url, form);
        }

        [When(@"the employer returns to the select apprentices page")]
        public async Task WhenTheEmployerReturnsToTheSelectApprenticesPage()
        {
            var hashedAccountId = _hashingService.HashValue(_data.AccountId);

            var url = $"{hashedAccountId}/apply/select-apprentices/{_data.ApplicationId}";

            _response = await _testContext.WebsiteClient.GetAsync(url);
            _response.EnsureSuccessStatusCode();
        }

        [Then(@"the user is directed to the confirmation page")]
        public void ThenTheUserIsDirectedToTheDeclarationPage()
        {
            var hashedAccountId = _hashingService.HashValue(_data.AccountId);
            _response.RequestMessage.RequestUri.PathAndQuery.Should().StartWith($"/{hashedAccountId}/apply/confirm-apprentices/");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationConfirmationViewModel;
            model.Should().NotBeNull();
            _response.Should().HaveBackLink($"/{hashedAccountId}/apply/select-apprentices/{model.ApplicationId}");
            model.Should().HaveTitle("Confirm apprentices");
        }

        [Then(@"the employer will receive an error")]
        public void ThenTheEmployerWillReceiveAnError()
        {
            var hashedAccountId = _hashingService.HashValue(_data.AccountId);
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            _response.Should().HaveTitle(model.Title);
            _response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/complete-apprentices/{_data.ApplicationId}");
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

            model.Apprenticeships.First(x => x.Id == _hashingService.HashValue(1)).Selected.Should().BeTrue();
            model.Apprenticeships.First(x => x.Id == _hashingService.HashValue(2)).Selected.Should().BeTrue();
            model.Apprenticeships.First(x => x.Id == _hashingService.HashValue(3)).Selected.Should().BeFalse();

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

            model.Apprenticeships.FirstOrDefault(x => x.Id == _hashingService.HashValue(99)).Should().BeNull();
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
