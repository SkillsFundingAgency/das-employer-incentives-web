using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.CompleteApplication
{
    [Binding]
    [Scope(Feature = "CompleteApplication")]
    public class CompleteApplicationSteps : StepsBase
    {
        private readonly TestContext _testContext;

        public CompleteApplicationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
        }

        [Given(@"the employer has entered all the information required to process their bank details")]
        public async Task GivenTheEmployerHasEnteredAllTheInformationRequiredToProcessTheirBankDetails()
        {
            var data = new TestData.Account.WithInitialApplicationAndBankingDetails();
            _testContext.TestDataStore.Add("HashedAccountId", data.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, data.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/applications/{data.ApplicationId}/accountlegalentity")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(data.AccountLegalEntityId.ToString()));

            var getBankingDetailsUrl = "/" + OuterApiRoutes.GetBankingDetailsUrl(data.AccountId, data.ApplicationId, data.HashedAccountId).Split("?").First();
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(getBankingDetailsUrl)
                        .WithParam("hashedAccountId", data.HashedAccountId)
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(data.BankingDetails, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{data.HashedAccountId}/bank-details/{data.ApplicationId}/enter-bank-details");

            var continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            continueNavigationResponse.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
            continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Contain("/service/provide-organisation-information?journey=new&return=https%3a%2f%2flocalhost%3a5001%2fapplication-complete&data=");
        }

        [When(@"the employer is shown the confirmation page")]
        public async Task WhenTheEmployerIsShownTheConfirmationPage()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/application-complete");

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"the employer has the option to return to their accounts page")]
        public void ThenTheEmployerHasTheOptionToReturnToTheirAccountsPage()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmationViewModel;
            model.Should().NotBeNull();
            model.AccountsUrl.Should().NotBeNullOrEmpty();
        }
    }
}
