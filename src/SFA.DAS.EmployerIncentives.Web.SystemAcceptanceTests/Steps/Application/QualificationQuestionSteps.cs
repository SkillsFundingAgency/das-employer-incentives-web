using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "QualificationQuestion")]
    public class QualificationQuestionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;

        public QualificationQuestionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testDataStore = _testContext.TestDataStore;
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
            response.EnsureSuccessStatusCode();
            _testContext.TestDataStore.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }        

        [Then(@"the employer is asked to select the apprenticeship")]
        public void ThenTheEmployerIsAskedWoSelectTheApprenticeship()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Select the apprentices you want to apply for");
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountLegalEntityId.Should().Be(hashedAccountLegalEntityId);

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/select-new-apprentices");            
        }

        [Then(@"the employer is informed that they cannot apply")]
        public void ThenTheEmployerIsInformedTheyCannotApply()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as CannotApplyViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("You cannot apply for this grant yet");
            model.AccountId.Should().Be(hashedAccountId);

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/cannot-apply");
        }

        [Then(@"the employer is informed that they need to specify whether or not they have qualifying apprenticeships")]
        public void ThenTheEmployerIsInformedTheyNeedToSelectAnOption()
        {
            var hashedAccountId = _testDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as QualificationQuestionViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Have you taken on new apprentices that joined your payroll after 1 August 2020?");
            model.AccountId.Should().Be(hashedAccountId);
            model.AccountLegalEntityId.Should().Be(hashedAccountLegalEntityId);

            response.Should().HaveTitle(model.Title);
            response.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedAccountLegalEntityId}/taken-on-new-apprentices");
        }
    }
}
