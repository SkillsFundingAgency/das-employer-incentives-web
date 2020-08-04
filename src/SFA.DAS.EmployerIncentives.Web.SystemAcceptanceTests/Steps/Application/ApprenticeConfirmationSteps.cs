﻿using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    public class ApprenticeConfirmationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private TestData.Account.WithInitialApplicationForASingleEntity _testData;

        public ApprenticeConfirmationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
        }

        [Given(@"an employer applying for a grant has already selected (.*) eligible apprentices")]
        public void GivenAnEmployerApplyingForAGrantHasAlreadySelectedEligibleApprentices(int p0)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();

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
                        .WithBody(JsonConvert.SerializeObject(_testData.GetApplicationResponse, TestHelper.DefaultSerialiserSettings)));
        }
        
        [When(@"the employer arrives on the confirm apprentices page")]
        public async Task WhenTheEmployerArrivesOnTheConfirmApprenticesPage()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_testData.HashedAccountId}/apply/confirm-apprentices/{_testData.ApplicationId}");

            var continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked to confirm the apprentices and expected amounts")]
        public void ThenTheEmployerIsAskedToConfirmTheApprenticesAndExpectedAmounts()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationConfirmationViewModel;
            model.Should().NotBeNull();

            var apiResponse = _testData.GetApplicationResponse;

            model.ApplicationId.Should().Be(_testData.ApplicationId);
            model.AccountId.Should().Be(_testContext.HashingService.HashValue(_testData.AccountId));
            model.AccountLegalEntityId.Should().Be(_testContext.HashingService.HashValue(_testData.AccountLegalEntityId));
            model.Apprentices.Count.Should().Be(apiResponse.Apprentices.Length);
            model.TotalPaymentAmount.Should().Be(apiResponse.Apprentices.Sum(x => x.ExpectedAmount));

            // For check the apprenticeships
            foreach (var apprentice in model.Apprentices)
            {
                var apiApprentice = apiResponse.Apprentices.First(x =>
                    x.ApprenticeshipId == _testContext.HashingService.DecodeValue(apprentice.ApprenticeshipId));

                apprentice.ApprenticeshipId.Should().Be(_testContext.HashingService.HashValue(apiApprentice.ApprenticeshipId));
                apprentice.DisplayName.Should().Be($"{apiApprentice.FirstName} {apiApprentice.LastName}");
                apprentice.CourseName.Should().Be(apiApprentice.CourseName);
                apprentice.ExpectedAmount.Should().Be(apiApprentice.ExpectedAmount);
            }
        }
    }
}
