using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using System;
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
    [Scope(Feature = "ViewApplications")]
    public class ViewApplicationsSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private Fixture _fixture;
        private TestData.Account.WithInitialApplicationForASingleEntity _testData;

        public ViewApplicationsSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _fixture = new Fixture();
        }

        [Given(@"an employer has a single submitted application")]
        public void GivenAnEmployerHasASingleSubmittedApplication()
        {
            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Create<ApprenticeApplicationModel>()
            };
            applications[0].Status = "Submitted";

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(applications)));
        }

        [When(@"the employer views their applications")]
        public async Task WhenTheEmployerViewsTheirApplications()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_testData.HashedAccountId}/payments/payment-applications");

            var response = await _testContext.WebsiteClient.SendAsync(request);
        }

        [Then(@"the employer is shown a single submitted application")]
        public void ThenTheEmployerIsShownASingleSubmittedApplication()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ViewApplicationsViewModel;
            model.Should().NotBeNull();
            model.Applications.Count().Should().Be(1);
        }

        [Given(@"an employer has multiple submitted applications")]
        public void GivenAnEmployerHasMultipleSubmittedApplications()
        {
            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Create<ApprenticeApplicationModel>(),
                _fixture.Create<ApprenticeApplicationModel>()
            };
            applications[0].Status = "Submitted";
            applications[1].Status = "Submitted";

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(applications)));
        }

        [Then(@"the employer is shown submitted applications")]
        public async Task ThenTheEmployerIsShownSubmittedApplications()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ViewApplicationsViewModel;
            model.Should().NotBeNull();
            model.Applications.Count().Should().Be(2);
        }

        [Given(@"an employer has submitted and in progress applications")]
        public void GivenAnEmployerHasSubmittedAndInProgressApplications()
        {
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(4));
            applications[0].Status = "Submitted";
            applications[1].Status = "Submitted";
            applications[2].Status = "Submitted";
            applications[3].Status = "InProgress";

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(applications)));
        }

        [Then(@"the employer is shown only submitted applications")]
        public void ThenTheEmployerIsShownOnlySubmittedApplications()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ViewApplicationsViewModel;
            model.Should().NotBeNull();
            model.Applications.Count().Should().Be(3);
        }


        [Given(@"an employer has in progress applications")]
        public void GivenAnEmployerHasInProgressApplications()
        {
            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Create<ApprenticeApplicationModel>(),
                _fixture.Create<ApprenticeApplicationModel>()
            };
            applications[0].Status = "InProgress";
            applications[1].Status = "InProgress";

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(applications)));
        }

        [Then(@"the employer is shown no applications")]
        public void ThenTheEmployerIsShownNoApplications()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ViewApplicationsViewModel;
            model.Should().NotBeNull();
            model.Applications.Count().Should().Be(0);
        }

        [Given(@"an employer has no applications")]
        public void GivenAnEmployerHasNoApplications()
        {
            var applications = new List<ApprenticeApplicationModel>();

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(applications)));
        }

    }
}
