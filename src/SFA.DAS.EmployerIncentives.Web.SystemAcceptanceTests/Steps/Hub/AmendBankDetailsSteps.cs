using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TechTalk.SpecFlow;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Hub
{
    [Binding]
    [Scope(Feature = "AmendBankDetails")]
    public class AmendBankDetailsSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestData.Account.WithInitialApplicationAndBankingDetails _testData;
        private HttpResponseMessage _response;

        public AmendBankDetailsSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = new TestData.Account.WithInitialApplicationAndBankingDetails();
        }

        [Given(@"an employer has submitted an application")]
        public void GivenAnEmployerHasSubmittedAnApplication()
        {
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);
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
                      .WithBody(JsonSerializer.Serialize(_testData.ApplicationResponse)));

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

            var getBankingDetailsUrl = "/" + OuterApiRoutes.Application.GetBankingDetailsUrl(_testData.AccountId, _testData.ApplicationId, _testData.HashedAccountId).Split("?").First();
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(getBankingDetailsUrl)
                        .WithParam("hashedAccountId", _testData.HashedAccountId)
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonSerializer.Serialize(_testData.BankingDetails, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

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
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(JsonSerializer.Serialize(_testData.LegalEntity)));
        }

        [When(@"the employer wishes to update their bank details")]
        public async Task WhenTheEmployerWishesToUpdateTheirBankDetails()
        {
            var url = $"{_testData.HashedAccountId}/bank-details/{_testData.ApplicationId}/change-bank-details";
            
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("AccountId", _testData.HashedAccountId),
                    new KeyValuePair<string, string>("AccountLegalEntityId", _testData.HashedAccountLegalEntityId),
                    new KeyValuePair<string, string>("ApplicationId", _testData.ApplicationId.ToString())
                })
            };

            _response = await _testContext.WebsiteClient.SendAsync(request);

            _response.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is shown the employer amend bank details journey")]
        public void ThenTheEmployerIsShownTheEmployerAmendBankDetailsJourney()
        {
            var redirectUrl = _response.RequestMessage.RequestUri.AbsoluteUri;
            redirectUrl.Should().Contain(_testContext.WebConfigurationOptions.AchieveServiceBaseUrl);
            redirectUrl.Should().Contain("journey=amend");
            var hubPageUrl = HttpUtility.UrlEncode($"https://localhost:5001/{_testData.HashedAccountId}/{_testData.HashedAccountLegalEntityId}/hire-new-apprentice-payment");
            redirectUrl.Should().Contain($"return={hubPageUrl}");
        }
    }
}
