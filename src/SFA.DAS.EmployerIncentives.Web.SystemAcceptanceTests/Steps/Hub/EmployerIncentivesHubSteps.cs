using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Hub;
using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using AutoFixture;
using System;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Hub
{
    [Binding]
    [Scope(Feature = "EmployerIncentivesHub")]
    public class EmployerIncentivesHubSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testDataStore;
        private readonly Fixture _fixture;
        private Guid _applicationId;

        public EmployerIncentivesHubSteps(TestContext testContext) : base (testContext)
        {
            _testContext = testContext;
            _testDataStore = testContext.TestDataStore;
            _fixture = new Fixture();
            _applicationId = Guid.NewGuid();
        }

        [Given(@"an employer has a single legal entity in their account")]
        public void GivenAnEmployerHasASingleLegalEntityInTheirAccount()
        {
            var testdata = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId);

            _testContext.EmployerIncentivesApi.MockServer
           .Given(
                   Request
                   .Create()
                   .WithPath($"/accounts/{testdata.AccountId}/legalentities")
                   .UsingGet()
                   )
               .RespondWith(
             Response.Create()
               .WithStatusCode(HttpStatusCode.OK)
               .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));
        }

        [When(@"the employer accesses the hub page")]
        public async Task WhenTheEmployerAccessesTheHubPage()
        {
            var accountId = _testDataStore.Get<string>("HashedAccountId");
            var accountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var url = $"{accountId}/{accountLegalEntityId}/hire-new-apprentice-payment";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            _testDataStore.Add<HttpResponseMessage>("Response", response);
        }

        [Then(@"they are presented with a link to apply for the employer incentive")]
        public void ThenTheyArePresentedWithALinkToApplyForTheEmployerIncentive()
        {
            var accountId = _testDataStore.Get<string>("HashedAccountId");
            var accountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HubPageViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Hire a new apprentice payment");
            model.AccountId.Should().Be(accountId);
            model.AccountLegalEntityId.Should().Be(accountLegalEntityId);

            response.Should().HaveTitle(model.Title);
            response.Should().HaveLink(".hub-apply-link", $"/{accountId}/{accountLegalEntityId}");
        }

        [Then(@"they are presented with a link to view previous applications")]
        public void ThenTheyArePresentedWithALinkToViewPreviousApplications()
        {
            var accountId = _testDataStore.Get<string>("HashedAccountId");
            var accountLegalEntityId = _testDataStore.Get<string>("HashedAccountLegalEntityId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");

            response.Should().HaveLink(".hub-payments-link", $"/{accountId}/payments/{accountLegalEntityId}/payment-applications");
        }

        [Then(@"the back link goes to the manage apprenticeships page")]
        public void ThenTheBackLinkGoesToTheManageApprenticeshipsPage()
        {
            var accountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");

            response.Should().HaveBackLink($"{_testContext.ExternalLinksOptions.ManageApprenticeshipSiteUrl}/accounts/{accountId}/teams");
        }

        [Given(@"an employer has a multiple legal entities in their account")]
        public void GivenAnEmployerHasAMultipleLegalEntitiesInTheirAccount()
        {
            var testdata = new TestData.Account.WithMultipleLegalEntitiesWithEligibleApprenticeships();
            _testDataStore.Add("HashedAccountId", testdata.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testdata.HashedAccountId);
            _testDataStore.Add("HashedAccountLegalEntityId", testdata.HashedAccountLegalEntityId1);

            _testContext.EmployerIncentivesApi.MockServer
           .Given(
                   Request
                   .Create()
                   .WithPath($"/accounts/{testdata.AccountId}/legalentities")
                   .UsingGet()
                   )
               .RespondWith(
             Response.Create()
               .WithStatusCode(HttpStatusCode.OK)
               .WithBody(JsonConvert.SerializeObject(testdata.LegalEntities, TestHelper.DefaultSerialiserSettings)));
        }

        [Given(@"the employer has previously supplied their bank details")]
        public void GivenTheEmployerHasPreviouslySuppliedTheirBankDetails()
        {
            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Create<ApprenticeApplicationModel>()
            };
            applications[0].Status = "Submitted";
            applications[0].ApplicationId = _applicationId;
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.Completed };

            var testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{testData.AccountId}/legalentity/{testData.AccountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));
        }

        [Given(@"the employer has not yet supplied bank details")]
        public void GivenTheEmployerHasNotYetSuppliedBankDetails()
        {
            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Create<ApprenticeApplicationModel>()
            };
            applications[0].Status = "Submitted";
            applications[0].ApplicationId = _applicationId;
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.NotSupplied };

            var testData = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testData.HashedAccountId);

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{testData.AccountId}/legalentity/{testData.AccountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));
        }

        [Then(@"the back link goes to the choose organisation page")]
        public void ThenTheBackLinkGoesToTheChooseOrganisationPage()
        {
            var accountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");

            response.Should().HaveBackLink($"/{accountId}/apply/choose-organisation");
        }

        [Then(@"they are presented with a banner link prompting them to enter their bank details")]
        public void ThenTheyArePresentedWithABannerLinkPromptingThemToEnterTheirBankDetails()
        {
            var accountId = _testDataStore.Get<string>("HashedAccountId");
            var response = _testDataStore.Get<HttpResponseMessage>("Response");

            response.Should().HaveLink(".hub-bank-details-link", $"/{accountId}/bank-details/{_applicationId}/add-bank-details");
        }
    }
}
