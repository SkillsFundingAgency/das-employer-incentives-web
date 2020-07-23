using AngleSharp.Html.Parser;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.HashingService;
using System.Collections.Generic;
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
    [Scope(Feature = "QualificationQuestion")]
    public class QualificationQuestionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;
        private readonly IHashingService _hashingService;

        public QualificationQuestionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
            _hashingService = _testContext.HashingService;
        }

        [Given(@"an employer applying for a grant has qualifying apprenticeships")]
        public void GivenAnEmployerApplyingHasQualifyingApprenticeships()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);
        }

        [Given(@"an employer applying for a grant does not have qualifying apprenticeships")]
        public void GivenAnEmployerApplyingDoesNotHaveQualifyingApprenticeships()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithNoEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);
        }

        [When(@"the employer specifies that they have qualifying apprenticeships")]
        public async Task WhenTheEmployerSelectsThatTheyHaveQualifyingApprenticeships()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("HasTakenOnNewApprentices", "true")
                })
            };

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }

        [When(@"the employer specifies that they do not have qualifying apprenticeships")]
        public async Task WhenTheEmployerSelectsThatTheyDoNotHaveQualifyingApprenticeships()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("HasTakenOnNewApprentices", "false")
                })
            };

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }

        [When(@"the employer does not specify whether or not they have qualifying apprenticeships")]
        public async Task WhenTheEmployerDoesNotSelectAQualifyingApprenticeshipsoption()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                })
            };

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }        

        [Then(@"the employer is asked to select the apprenticeship")]
        public async Task ThenTheEmployerIsAskedWoSelectTheApprenticeship()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            response.EnsureSuccessStatusCode();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("Select Apprenticeships");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/select-new-apprentices");
        }

        [Then(@"the employer is informed that they cannot apply")]
        public async Task ThenTheEmployerIsInformedTheyCannotApply()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");

            response.EnsureSuccessStatusCode();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("You cannot apply for this grant yet");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/cannot-apply");
        }

        [Then(@"the employer is informed that they need to specify whether or not they have qualifying apprenticeships")]
        public async Task ThenTheEmployerIsInformedTheyNeedToSelectAnOption()
        {
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");

            response.EnsureSuccessStatusCode();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("Have you taken on new apprentices that joined your payroll after 1 August 2020?");
            response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices");

            // todo get viewmodel and check for error
        }
    }
}
