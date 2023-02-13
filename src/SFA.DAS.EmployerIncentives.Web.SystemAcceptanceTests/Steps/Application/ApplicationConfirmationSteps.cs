using AutoFixture;
using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "ApplicationConfirmation")]
    public class ApplicationConfirmationSteps : StepsBase
    {
        private const string ReadyToEnterBankDetailsUrl = "/need-bank-details";
        private readonly TestContext _testContext;
        private HttpResponseMessage _continueNavigationResponse;
        private readonly TestData.Account.WithInitialApplicationForASingleEntity _testData;
        private Fixture _fixture;

        public ApplicationConfirmationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);
            _fixture = new Fixture();
        }

        [Given(@"an employer applying for a grant is asked to agree a declaration")]
        public void GivenAnEmployerApplyingForAGrantHasAlreadyConfirmedSelectedApprentices()
        {
            SetUpApiResponses(HttpStatusCode.OK);
        }

        [Given(@"an employer applying for a grant for an already submitted uln")]
        public void GivenAnEmployerApplyingForAGrantForAnAlreadySubmittedUln()
        {
            SetUpApiResponses(HttpStatusCode.Conflict);
        }

        [When(@"the employer understands and confirms the declaration")]
        public async Task WhenTheEmployerUnderstandsAndConfirmsTheDeclaration()
        {

            var application = _fixture.Create<IncentiveApplicationDto>();
            application.AccountLegalEntityId = _testData.AccountLegalEntityId;
            application.BankDetailsRequired = true;
            var response = new ApplicationResponse { Application = application };

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                  Request
                      .Create()
                      .WithPath($"/accounts/{_testData.AccountId}/applications/{_testData.ApplicationId}")
                      .WithParam("includeApprenticeships", new ExactMatcher("False"))
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(JsonSerializer.Serialize(response)));
            
            var url = $"{_testData.HashedAccountId}/apply/declaration/{_testData.ApplicationId}/";
            var formData = new KeyValuePair<string, string>();
            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url, formData);
        }

        [Then(@"the employer application declaration is accepted")]
        public void ThenTheApprenticeshipApplicationIsSubmittedAndSaved()
        {
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked to enter bank details")]
        public void ThenTheEmployerIsAskedToEnterBankDetails()
        {
            var expectedUrl = $"/{_testData.HashedAccountId}/bank-details/{_testData.ApplicationId}{ReadyToEnterBankDetailsUrl}";
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be(expectedUrl);
            var viewResult = _testContext.ActionResult.LastViewResult.Model as BankDetailsConfirmationViewModel;
            viewResult.Should().NotBeNull();
            viewResult.AccountId.Should().Be(_testData.HashedAccountId);
            viewResult.ApplicationId.Should().Be(_testData.ApplicationId);
        }

        [Then(@"the employer is shown the uln already submitted page")]
        public void ThenTheEmployerIsShownTheUlnAlreadySubmittedPage()
        {
            var expectedUrl = $"/{_testData.HashedAccountId}/apply/{_testData.HashedAccountLegalEntityId}/problem-with-service";
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be(expectedUrl);
        }

        private void SetUpApiResponses(HttpStatusCode submitStatusCode)
        {
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Created));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications")
                        .UsingPatch()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(submitStatusCode));

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
        }
    }
}
