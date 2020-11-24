using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Payments
{
    [Binding]
    [Scope(Feature = "ChooseOrganisation")]
    public class ChooseOrganisationSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private Fixture _fixture;
        private long _accountId;
        private string _hashedAccountId;
        private HttpResponseMessage _response;

        public ChooseOrganisationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _fixture = new Fixture();

            var testData = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _accountId = testData.AccountId;
            _hashedAccountId = testData.HashedAccountId;

            _testContext.TestDataStore.Add("HashedAccountId", testData.HashedAccountId);
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, testData.HashedAccountId);
        }

        [Given(@"the employer account has more than one organisation")]
        public void GivenTheEmployerAccountHasMoreThanOneOrganisation()
        {
            var legalEntities = new List<LegalEntityDto>
            {
                new LegalEntityDto { AccountId = _accountId, AccountLegalEntityId = _fixture.Create<long>() },
                new LegalEntityDto { AccountId = _accountId, AccountLegalEntityId = _fixture.Create<long>() }
            };
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_accountId}/legalentities")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(legalEntities)));
        }

        [When(@"viewing payments")]
        public async Task WhenViewingPayments()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_hashedAccountId}/payments/payment-applications");

            _response = await _testContext.WebsiteClient.SendAsync(request);
        }

        [Then(@"the user is prompted to choose the organisation to view payments for")]
        public void ThenTheUserIsPromptedToChooseTheOrganisationToViewPaymentsFor()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _response.RequestMessage.RequestUri.LocalPath.Should().Contain("choose-organisation");
        }

        [Given(@"the employer account has a single organisation")]
        public void GivenTheEmployerAccountHasASingleOrganisation()
        {
            var accountLegalEntityId = _fixture.Create<long>();

            var legalEntities = new List<LegalEntityDto>
            {
                new LegalEntityDto { AccountId = _accountId, AccountLegalEntityId = accountLegalEntityId }
            };
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_accountId}/legalentities")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(legalEntities)));

            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Create<ApprenticeApplicationModel>()
            };
            applications[0].Status = "Submitted";
            var getApplications = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.NotSupplied };

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_accountId}/legalentity/{accountLegalEntityId}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplications)));

        }

        [Then(@"the payments for that organisation are shown")]
        public void ThenThePaymentsForThatOrganisationAreShown()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _response.RequestMessage.RequestUri.LocalPath.Should().Contain("payment-applications");
        }

    }
}
