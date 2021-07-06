﻿using AutoFixture;
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
            AnEmployerHasASingleSubmittedApplication(_testApplicationId);
        }        
        
        [Given(@"an employer with accepted bank details has a single submitted application")]
        public void GivenAnEmployerWithAcceptedBankDetailsHasASingleSubmittedApplication()
        {
            AnEmployerHasASingleSubmittedApplication(_testApplicationId, BankDetailsStatus.Completed);
        }

        [Given(@"an employer with a later agreement version that needs signing")]
        public void GivenAnEmployerWithALaterAgreementVersionThatNeedsSigning()
        {
            AnEmployerHasAnApplicationWithAnAgreementVersionThatNeedsSigning(_testApplicationId);
        }
        [Given(@"an employer with an agreement version that has been signed")]
        public void GivenAnEmployerWithALaterAgreementVersionThatHasBeenSigned()
        {
            AnEmployerHasAnApplicationWithAnAgreementVersionThatDoesNotNeedSigning(_testApplicationId);
        }

        [Given(@"an employer with a stopped application")]
        public void GivenAnEmployerWithAStoppedApplication()
        {
            AnEmployerWithAStoppedApplication(_testApplicationId);
        }

        [Given(@"an employer without bank details has a single submitted application")]
        public void GivenAnEmployerWithoutBankDetailsHasASingleSubmittedApplication()
        {
            AnEmployerHasASingleSubmittedApplication(_testApplicationId, BankDetailsStatus.NotSupplied);
        }

        [Given(@"an employer with vrf rejected status has a single submitted application")]
        public void GivenAnEmployerWithVrfRejectedStatusHasASingleSubmittedApplication()
        {
            AnEmployerHasASingleSubmittedApplication(_testApplicationId, BankDetailsStatus.Rejected);
        }

        [Given(@"an employer with an application withdrawn by compliance")]
        public void GivenAnEmployerWithAnApplicationWithdrawnByCompliance()
        {
            AnEmployerWithAnApplicationWithdrawnByCompliance(_testApplicationId);
        }

        [Given(@"an employer has withdrawn an application")]
        public void GivenAnEmployerHasWithdrawnAnApplication()
        {
            AnEmployerHasWithdrawnAnApplication(_testApplicationId);
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

            var hashedAccountId = _testContext.TestDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testContext.TestDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().HaveBackLink($"/{hashedAccountId}/{hashedAccountLegalEntityId}/hire-new-apprentice-payment");
        }

        [Given(@"an employer has multiple submitted applications")]
        public void GivenAnEmployerHasMultipleSubmittedApplications()
        {
            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Create<ApprenticeApplicationModel>(),
                _fixture.Create<ApprenticeApplicationModel>()
            };
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.InProgress };

            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
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

            var hashedAccountId = _testContext.TestDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testContext.TestDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().HaveBackLink($"/{hashedAccountId}/{hashedAccountLegalEntityId}/hire-new-apprentice-payment");
        }

        [Then(@"the add bank details call to action is shown")]
        public void ThenTheAddBankDetailsCalltoActionIsShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");

            response.Should().HaveLink("[data-linktype='add-bank-details']", $"https://{response.RequestMessage.RequestUri.Authority}/{_testData.HashedAccountId}/bank-details/{_testApplicationId}/add-bank-details");
        }

        [Then(@"the add bank details call to action is not shown")]
        public void ThenTheAddBankDetailsCalltoActionIsNotShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");

            response.Should().NotHaveLink($"https://{response.RequestMessage.RequestUri.Authority}/{_testData.HashedAccountId}/bank-details/{_testApplicationId}/add-bank-details");
        }

        [Then(@"the accept new employer agreement call to action is shown")]
        public void ThenTheShowAcceptNewEmployerAgreementIsShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().HaveLink("[data-linktype='view-agreement']", $"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{_testData.HashedAccountId}/agreements");
            response.Should().HaveLink("[data-linktype='payment-status-view-agreement']", $"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{_testData.HashedAccountId}/agreements");            
        }

        [Then(@"the message showing the application is stopped is shown")]
        public void ThenTheMessageShowingTheApplicationisStoppedIsShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");            
            response.Should().HaveInnerHtml("[data-paragraphtype='view-agreement-stopped']", $"Apprenticeship paused or stopped");
        }

        [Then(@"the message showing the application is rejected is shown")]
        public void ThenTheMessageShowingTheApplicationisRejectedIsShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().HaveInnerHtml("[data-paragraphtype='view-agreement-withdrawnByCompliance']", $"Application rejected");
        }

        [Then(@"the message showing the application is cancelled is shown")]
        public void ThenTheMessageShowingTheApplicationisCancelledIsShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().HaveInnerHtml("[data-paragraphtype='view-agreement-withdrawnByEmployer']", $"Application cancelled");
        }

        [Then(@"the accept new employer agreement call to action is not shown")]
        public void ThenTheShowAcceptNewEmployerAgreementIsNotShown()
        {
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().NotHaveLink("[data-linktype='view-agreement']", $"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{_testData.HashedAccountId}/agreements");
            response.Should().NotHaveLink("[data-linktype='payment-status-view-agreement']", $"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{_testData.HashedAccountId}/agreements");
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
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
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

            var hashedAccountId = _testContext.TestDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testContext.TestDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().HaveBackLink($"/{hashedAccountId}/{hashedAccountLegalEntityId}/hire-new-apprentice-payment");
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
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
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
            
            var hashedAccountId = _testContext.TestDataStore.Get<string>("HashedAccountId");
            var hashedAccountLegalEntityId = _testContext.TestDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testContext.TestDataStore.Get<HttpResponseMessage>("Response");
            response.Should().HaveBackLink($"/{hashedAccountId}/{hashedAccountLegalEntityId}/hire-new-apprentice-payment");
        }

        [Given(@"an employer has no applications")]
        public void GivenAnEmployerHasNoApplications()
        {
            var applications = new List<ApprenticeApplicationModel>();
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.NotSupplied };
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
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

        private void AnEmployerHasASingleSubmittedApplication(Guid applicationId, BankDetailsStatus bankDetailsStatus = BankDetailsStatus.Completed)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.AccountId, _testData.AccountId)
                .Create()
            };
            applications[0].Status = "Submitted";
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = bankDetailsStatus, FirstSubmittedApplicationId = applicationId };

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

        private void AnEmployerHasAnApplicationWithAnAgreementVersionThatNeedsSigning(Guid applicationId)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.AccountId, _testData.AccountId)
                .With(p => p.FirstPaymentStatus, 
                    _fixture.Build<PaymentStatusModel>()
                    .With(p => p.RequiresNewEmployerAgreement, true)
                    .With(p => p.PaymentIsStopped, false)
                    .With(p => p.WithdrawnByCompliance, false)
                    .With(p => p.WithdrawnByEmployer, false)
                    .Create()
                    )
                .Create()
            };

            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, FirstSubmittedApplicationId = applicationId };

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

        private void AnEmployerHasAnApplicationWithAnAgreementVersionThatDoesNotNeedSigning(Guid applicationId)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.AccountId, _testData.AccountId)
                .With(p => p.FirstPaymentStatus,
                    _fixture.Build<PaymentStatusModel>()
                    .With(p => p.RequiresNewEmployerAgreement, false)
                    .With(p => p.WithdrawnByCompliance, false)
                    .With(p => p.WithdrawnByEmployer, false)
                    .Create()
                    )
                .Create()
            };

            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, FirstSubmittedApplicationId = applicationId };

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

        private void AnEmployerWithAStoppedApplication(Guid applicationId)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.AccountId, _testData.AccountId)
                .With(p => p.FirstPaymentStatus,
                    _fixture.Build<PaymentStatusModel>()
                    .With(p => p.PaymentIsStopped, true)

                    .Create()
                    )
                .Create()
            };

            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, FirstSubmittedApplicationId = applicationId };

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

        private void AnEmployerWithAnApplicationWithdrawnByCompliance(Guid applicationId)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(p => p.AccountId, _testData.AccountId)
                    .With(p => p.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(p => p.WithdrawnByCompliance, true)
                            .With(p => p.WithdrawnByEmployer, false)
                            .With(p => p.PaymentIsStopped, false)
                            .Create()
                    )
                    .Create()
            };

            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, FirstSubmittedApplicationId = applicationId };

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

        private void AnEmployerHasWithdrawnAnApplication(Guid applicationId)
        {
            _testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.TestDataStore.Add("HashedAccountId", _testData.HashedAccountId);
            _testContext.TestDataStore.Add("HashedAccountLegalEntityId", _testData.HashedAccountLegalEntityId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _testData.HashedAccountId);

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(p => p.AccountId, _testData.AccountId)
                    .With(p => p.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(p => p.WithdrawnByEmployer, true)
                            .With(p => p.WithdrawnByCompliance, true)
                            .With(p => p.PaymentIsStopped, false)
                            .Create()
                    )
                    .Create()
            };

            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, FirstSubmittedApplicationId = applicationId };

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
