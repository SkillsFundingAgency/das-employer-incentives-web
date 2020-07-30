using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    public class ApplicationConfirmationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private HttpResponseMessage _continueNavigationResponse;
        private readonly TestData.Account.WithInitialApplicationForASingleEntity _testData;

        public ApplicationConfirmationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
        }

        [Given(@"an employer applying for a grant is asked to agree a declaration")]
        public void GivenAnEmployerApplyingForAGrantHasAlreadyConfirmedSelectedApprentices()
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
                        .WithPath($"/accounts/{_testData.AccountId}/confirm-application/{_testData.ApplicationId}")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Created));

        }

        [When(@"the employer understands and confirms the declaration")]
        public async Task WhenTheEmployerUnderstandsAndConfirmsTheDeclaration()
        {
            var url = $"{_testData.HashedAccountId}/apply/declaration/{_testData.ApplicationId}/";
            var formData = new KeyValuePair<string, string>();
            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url, formData);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"then the employer application declaration is accepted")]
        public void ThenTheApprentishipApplicationIsSubmittedAndSaved()
        {
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().StartWith($"/{_testData.HashedAccountId}/apply/bank-details");
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
        }

    }
}
