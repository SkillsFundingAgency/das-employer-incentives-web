using AngleSharp.Html.Parser;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "ReadyToEnterBankDetails")]
    public class ReadyToEnterBankDetailsSteps : StepsBase
    {
        private const string ReadyToEnterBankDetailsUrl = "/need-bank-details";
        private const string NeedBankDetailsUrl = "/complete/need-bank-details";
        private const string ApplicationCompleteUrl = "/application-saved";

        private readonly TestContext _testContext;
        private HttpResponseMessage _continueNavigationResponse;
        private readonly TestData.Account.WithInitialApplicationAndBankingDetails _data;
        private readonly Fixture _fixture;
        private IncentiveApplicationDto _application;

        public ReadyToEnterBankDetailsSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _fixture = new Fixture();
            _data = new TestData.Account.WithInitialApplicationAndBankingDetails();

            _testContext.TestDataStore.Add("HashedAccountId", _data.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _data.HashedAccountId);
        }


        [When(@"the employer has confirmed their apprenticeship details")]
        public async Task WhenTheEmployerHasConfirmedTheirApprenticeshipDetails()
        {
            var url = $"{_data.HashedAccountId}/bank-details/{_data.ApplicationId}{ReadyToEnterBankDetailsUrl}";
            var accountLegalEntityId = _fixture.Create<long>();

            var response = new ApplicationResponse { Application = _application };

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                  Request
                      .Create()
                      .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}")
                      .WithParam("includeApprenticeships", new ExactMatcher("False"))
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(JsonConvert.SerializeObject(response)));

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
                      .WithBody(accountLegalEntityId.ToString()));


            _continueNavigationResponse = await _testContext.WebsiteClient.GetAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked whether they can provide their organisation's bank details now")]
        public async Task ThenTheEmployerIsAskedWhetherTheyCanProvideTheirOrganisationSBankDetailsNow()
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await _continueNavigationResponse.Content.ReadAsStreamAsync());

            document.Title.Should().Be(new BankDetailsConfirmationViewModel().Title);
        }

        [When(@"the employer confirms they can provide their bank details")]
        public async Task WhenTheEmployerConfirmsTheyCanProvideTheirBankDetails()
        {

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
                      .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}")
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(JsonConvert.SerializeObject(_data.ApplicationResponse))
                      .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
             .Given(
                 Request
                     .Create()
                     .WithPath($"/email/bank-details-reminder")
                     .UsingPost()
             )
             .RespondWith(
                 Response.Create()
                     .WithStatusCode(HttpStatusCode.OK)
                     .WithHeader("Content-Type", "application/json")
                     .WithBody(string.Empty));

            var getBankingDetailsUrl = "/" + OuterApiRoutes.Application.GetBankingDetailsUrl(_data.AccountId, _data.ApplicationId, _data.HashedAccountId).Split("?").First();
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(getBankingDetailsUrl)
                        .WithParam("hashedAccountId", _data.HashedAccountId)
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_data.BankingDetails, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            var url = $"{_data.HashedAccountId}/bank-details/{_data.ApplicationId}{ReadyToEnterBankDetailsUrl}";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("canProvideBankDetails", "true")
                    })
            };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
        }

        [Then(@"the employer is requested to enter their bank details")]
        public void ThenTheEmployerIsRedirectedToTheEnterBankDetailsPage()
        {
            _continueNavigationResponse.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Contain($"/{_data.HashedAccountId}/bank-details/{_data.ApplicationId}/add-bank-details");
        }

        [When(@"the employer states that they are unable to provide bank details now")]
        public async Task WhenTheEmployerStatesThatTheyAreUnableToProvideBankDetailsNow()
        {
            var accountLegalEntityId = _fixture.Create<long>();
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
                       .WithBody(accountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
               .Given(
                   Request
                       .Create()
                       .WithPath($"/email/bank-details-required")
                       .UsingPost()
               )
               .RespondWith(
                   Response.Create()
                       .WithStatusCode(HttpStatusCode.OK)
                       .WithHeader("Content-Type", "application/json")
                       .WithBody(string.Empty));


            var url = $"{_data.HashedAccountId}/bank-details/{_data.ApplicationId}{ReadyToEnterBankDetailsUrl}";

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


            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("canProvideBankDetails", "false")
                })
            };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is requested to enter their bank details at a later date")]
        public void ThenTheEmployerIsRedirectedToTheReadyToEnterBankDetailsPage()
        {
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Contain(NeedBankDetailsUrl);
        }

        [When(@"the employer does not confirm whether they can provide bank details now")]
        public async Task WhenTheEmployerDoesNotConfirmWhetherTheyCanProvideBankDetailsNow()
        {
            var url = $"{_data.HashedAccountId}/bank-details/{_data.ApplicationId}{ReadyToEnterBankDetailsUrl}";
            var accountLegalEntityId = _fixture.Create<long>();

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
                      .WithBody(accountLegalEntityId.ToString()));


            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>())
            };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is prompted to confirm with an answer")]
        public async Task ThenTheEmployerIsPromptedToConfirmWithAnAnswer()
        {
            _continueNavigationResponse.RequestMessage.RequestUri.LocalPath.Should().Contain(ReadyToEnterBankDetailsUrl);

            var parser = new HtmlParser();
            var document = parser.ParseDocument(await _continueNavigationResponse.Content.ReadAsStreamAsync());
            document.Title.Should().Be(new BankDetailsConfirmationViewModel().Title);

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as BankDetailsConfirmationViewModel;
            model.Should().NotBeNull();
            model.CanProvideBankDetails.Should().BeNull();
            viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            viewResult.ViewData.ModelState.ContainsKey(nameof(BankDetailsConfirmationViewModel.CanProvideBankDetails)).Should().BeTrue();
            viewResult.ViewData.ModelState[nameof(BankDetailsConfirmationViewModel.CanProvideBankDetails)]
                .Errors.First().ErrorMessage.Should().Be(new BankDetailsConfirmationViewModel().CanProvideBankDetailsNotSelectedMessage);
        }

        [Then(@"the employer is sent an email reminding them to supply their bank details to complete the application")]
        public void ThenTheEmployerIsSentAnEmailRemindingThemToSupplyTheirBankDetailsToCompleteTheApplication()
        {
            var emailRequests = _testContext.EmployerIncentivesApi.MockServer.FindLogEntries(Request.Create().WithPath($"/email/bank-details-required").UsingPost());
            emailRequests.Should().HaveCount(1);
        }

        [Then(@"the employer is sent an email with details of how to enter their bank details in case they are unable to complete the journey")]
        public void ThenTheEmployerIsSentAnEmailWithDetailsOfHowToEnterTheirBankDetailsInCaseTheyAreUnableToCompleteTheJourney()
        {
            var emailRequests = _testContext.EmployerIncentivesApi.MockServer.FindLogEntries(Request.Create().WithPath($"/email/bank-details-reminder").UsingPost());
            emailRequests.Should().HaveCount(1);
        }

        [When(@"the employer has not previously supplied bank details")]
        public void WhenTheEmployerHasNotPreviouslySuppliedBankDetails()
        {
            _application = _fixture.Build<IncentiveApplicationDto>().With(p => p.BankDetailsRequired, true).Create();
            var response = new ApplicationResponse { Application = _application };

            _testContext.EmployerIncentivesApi.MockServer.ResetMappings();
            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                  Request
                      .Create()
                      .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}")
                      .WithParam("includeApprenticeships", new ExactMatcher("False"))
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(JsonConvert.SerializeObject(response)));
        }

        [When(@"the employer has already provided bank details")]
        public void WhenTheEmployerHasAlreadyProvidedBankDetails()
        {
            _application = _fixture.Build<IncentiveApplicationDto>().With(p => p.BankDetailsRequired, false).Create();
            var response = new ApplicationResponse { Application = _application };

            _testContext.EmployerIncentivesApi.MockServer.ResetMappings();
            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                  Request
                      .Create()
                      .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}")
                      .WithParam("includeApprenticeships", new ExactMatcher("False"))
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(JsonConvert.SerializeObject(response)));
        }

        [Then(@"the employer is shown the application complete page")]
        public void ThenTheEmployerIsShownTheApplicationCompletePage()
        {
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Contain(ApplicationCompleteUrl);
        }

    }
}
