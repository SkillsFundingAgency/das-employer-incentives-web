using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
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

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Payments
{
    [Binding]
    [Scope(Feature = "ViewApplications")]
    public class ViewApplicationsSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private Fixture _fixture;
        private TestData.Account.WithInitialApplicationForASingleEntity _testData;
        private Guid _testApplicationId;        

        public ViewApplicationsSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _fixture = new Fixture();
            _testApplicationId = Guid.NewGuid();
        }

        [Given(@"an employer has a single submitted application")]
        public void GivenAnEmployerHasASingleSubmittedApplication()
        {
            AnEmployerHasASingleSubmittedApplication();
        }        
        
        [Given(@"an employer with accepted bank details has a single submitted application")]
        public void GivenAnEmployerWithAcceptedBankDetailsHasASingleSubmittedApplication()
        {
            AnEmployerHasASingleSubmittedApplication(BankDetailsStatus.Completed);
        }

        [Given(@"an employer without bank details has a single submitted application")]
        public void GivenAnEmployerWithoutBankDetailsHasASingleSubmittedApplication()
        {
            AnEmployerHasASingleSubmittedApplication(BankDetailsStatus.NotSupplied);
        }

        [Given(@"an employer with vrf rejected status has a single submitted application")]
        public void GivenAnEmployerWithVrfRejectedStatusHasASingleSubmittedApplication()
        {
            AnEmployerHasASingleSubmittedApplication(BankDetailsStatus.Rejected);
        }

        [When(@"the employer views their applications")]
        public async Task WhenTheEmployerViewsTheirApplications()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_testData.HashedAccountId}/payments/{_testData.HashedAccountLegalEntityId}/payment-applications");

            var response = await _testContext.WebsiteClient.SendAsync(request);

            _testContext.TestDataStore.GetOrCreate("Response", onCreate: () =>
            {
                return response;
            });
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
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.InProgress };

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/legalentity/{_testData.AccountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));
        }

        [Then(@"the employer is shown submitted applications")]
        public void ThenTheEmployerIsShownSubmittedApplications()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ViewApplicationsViewModel;
            model.Should().NotBeNull();
            model.Applications.Count().Should().Be(2);
        }

        [Then(@"the add bank details call to action is shown")]
        public void ThenTheAddBankDetailsCalltoActionIsShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");

            response.Should().HaveLink($"https://{response.RequestMessage.RequestUri.Authority}/{_testData.HashedAccountId}/bank-details/{_testApplicationId}/add-bank-details");
        }

        [Then(@"the add bank details call to action is not shown")]
        public void ThenTheAddBankDetailsCalltoActionIsNotShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");

            response.Should().NotHaveLink($"https://{response.RequestMessage.RequestUri.Authority}/{_testData.HashedAccountId}/bank-details/{_testApplicationId}/add-bank-details");
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
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.InProgress };

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/legalentity/{_testData.AccountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));
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
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.NotSupplied };

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/legalentity/{_testData.AccountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));
        }

        [Then(@"the employer is shown no applications")]
        public void ThenTheEmployerIsShownNoApplications()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as NoApplicationsViewModel;
            model.Should().NotBeNull();
        }

        [Given(@"an employer has no applications")]
        public void GivenAnEmployerHasNoApplications()
        {
            var applications = new List<ApprenticeApplicationModel>();
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.NotSupplied };
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/legalentity/{_testData.AccountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));
        }

        [Given(@"an employer account has multiple legal entities")]
        public void GivenAnEmployerAccountHasMultipleLegalEntities()
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            var legalEntities = new List<LegalEntityModel>
            {
                new LegalEntityModel { AccountId = _testData.AccountId.ToString(), AccountLegalEntityId =_testData.AccountLegalEntityId.ToString() },
                new LegalEntityModel { AccountId = _testData.AccountId.ToString(), AccountLegalEntityId = _fixture.Create<string>() }
            };
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
                        .WithBody(JsonConvert.SerializeObject(legalEntities)));
        }

        private void AnEmployerHasASingleSubmittedApplication(BankDetailsStatus bankDetailsStatus = BankDetailsStatus.Completed)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.AccountId, _testData.AccountId)
                .With(p => p.ApplicationId, _testApplicationId)
                .Create()
            };
            applications[0].Status = "Submitted";
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = bankDetailsStatus };


            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_testData.AccountId}/legalentity/{_testData.AccountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));

        }

    }
}
