using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
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
    [Scope(Feature = "ApprenticeConfirmation")]
    public class ApprenticeConfirmationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestData.Account.WithInitialApplicationForASingleEntity _testData;
        private HttpResponseMessage _continueNavigationResponse;
        private readonly bool _newAgreementRequired = false;

        public ApprenticeConfirmationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);
        }

        [Given(@"an employer applying for a grant has already selected (.*) eligible apprentices")]
        public void GivenAnEmployerApplyingForAGrantHasAlreadySelectedEligibleApprentices(int p0)
        {
            SetupServiceMocks(_testData.ApplicationResponse);
        }

        [Given(@"an employer has selected an apprentice within the extension window and has signed the extension agreement")]
        public void GivenAnEmployerHasSelectedAnApprenticeWithinTheExtensionWindowAndHasSignedTheAgreement()
        {
            _testData.ApplicationResponse.Application.NewAgreementRequired = false;
            SetupServiceMocks(_testData.GetApplicationResponseWithFirstTwoApprenticesSelected);
        }

        [Given(@"a initial application has been created and submitted")]
        public void GivenAInitialApplicationHasBeenCreatedAndSubmitted()
        {
            var response = _testData.ApplicationResponse;
            response.Application.SubmittedByEmail = "SubmittedBy@test.co.uk";
            SetupServiceMocks(response);
        }

        [When(@"the employer arrives on the confirm apprentices page")]
        public async Task WhenTheEmployerArrivesOnTheConfirmApprenticesPage()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_testData.HashedAccountId}/apply/confirm-apprentices/{_testData.ApplicationId}");

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked to confirm the apprentices and expected amounts")]
        public void ThenTheEmployerIsAskedToConfirmTheApprenticesAndExpectedAmounts()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationConfirmationViewModel;
            model.Should().NotBeNull();

            var apiResponse = _testData.ApplicationResponse;

            model.ApplicationId.Should().Be(_testData.ApplicationId);
            model.AccountId.Should().Be(_testContext.EncodingService.Encode(_testData.AccountId));
            model.AccountLegalEntityId.Should().Be(_testContext.EncodingService.Encode(_testData.AccountLegalEntityId));
            model.Apprentices.Count.Should().Be(apiResponse.Application.Apprenticeships.Count());
            model.TotalPaymentAmount.Should().Be(apiResponse.Application.Apprenticeships.Sum(x => x.TotalIncentiveAmount));

            // Check the apprenticeships
            var apprentice = model.Apprentices.First();
            var apiApprentice = apiResponse.Application.Apprenticeships.OrderBy(x=>x.LastName).First();

            apprentice.DisplayName.Should().Be($"{apiApprentice.FirstName} {apiApprentice.LastName}");
            apprentice.CourseName.Should().Be(apiApprentice.CourseName);
            apprentice.ExpectedAmount.Should().Be(apiApprentice.TotalIncentiveAmount);
        }

        [When(@"the employer confirms their selection")]
        public async Task WhenTheEmployerConfirmsTheirSelection()
        {
            var url = $"{_testData.HashedAccountId}/apply/confirm-apprentices/{_testData.ApplicationId}/";
            var formData = new KeyValuePair<string, string>("newAgreementRequired", _newAgreementRequired.ToString());
            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url, formData);
        }

        [Then(@"the employer is asked to read and accept a declaration")]
        public void ThenTheEmployerIsAskedToReadAndAcceptADeclaration()
        {
            _continueNavigationResponse.EnsureSuccessStatusCode();
            _continueNavigationResponse.Should().HavePathAndQuery($"/{_testData.HashedAccountId}/apply/declaration/{_testData.ApplicationId}");

            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as DeclarationViewModel;
            model.Should().NotBeNull();
            model.Title.Should().Be("Declaration");
            model.ApplicationId.Should().Be(_testData.ApplicationId);
            model.AccountId.Should().Be(_testData.HashedAccountId);
            _continueNavigationResponse.Should().HaveBackLink($"/{_testData.HashedAccountId}/apply/confirm-apprentices/{_testData.ApplicationId}?all=false");
        }

        [Then(@"the user is directed to the hub page")]
        public void ThenTheUserIsDirectedToTheHubPage()
        {
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_testData.HashedAccountId}/{_testData.HashedAccountLegalEntityId}/hire-new-apprentice-payment");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HubPageViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Hire a new apprentice payment");
        }

        private void SetupServiceMocks(ApplicationResponse applicationResponse)
        {
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications/{_testData.ApplicationId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonSerializer.Serialize(applicationResponse,
                            TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications/{_testData.ApplicationId}/accountlegalentity")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_testData.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/legalentities/{_testData.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonSerializer.Serialize(_testData.LegalEntity, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
             .Given(
                     Request
                     .Create()
                     .WithPath($"/accounts/{_testData.AccountId}/legalentities")
                     .UsingGet()
                     )
                 .RespondWith(
             Response.Create()
                 .WithStatusCode(HttpStatusCode.OK)
                 .WithBody(JsonSerializer.Serialize(_testData.LegalEntities, TestHelper.DefaultSerialiserSettings)));

        }
    }
}