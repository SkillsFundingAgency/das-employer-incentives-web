using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "EmploymentStartDates")]
    public class EmploymentStartDatesSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly IHashingService _hashingService;
        private TestData.Account.WithInitialApplicationForASingleEntity _data;
        private HttpResponseMessage _response;
        private LegalEntityDto _legalEntity;

        public EmploymentStartDatesSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _hashingService = _testContext.HashingService;
            _data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _legalEntity = _data.LegalEntities.First();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _data.HashedAccountId);
        }

        [Given(@"an initial application has been created")]
        public void GivenAnInitialApplicationHasBeenCreated()
        {
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonConvert.SerializeObject(_data.GetApplicationResponseWithFirstTwoApprenticesSelected, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}/accountlegalentity")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_data.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/legalentities/{_legalEntity.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_legalEntity))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Created)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(string.Empty));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x =>
                            x.Contains($"accounts/{_data.AccountId}/applications") &&
                            x.Contains("accountlegalentity")) // applicationid is generated in application service so will vary per request
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_data.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications/{_data.ApplicationId}/employmentDetails"))
                        .UsingPatch()
                    )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                    );
        }

        [When(@"the employer has selected apprentices for the application")]
        public async Task WhenTheEmployerHasSelectedApprenticesForTheApplication()
        {
            var apprenticeships = _data.Apprentices.ToApprenticeshipModel(_hashingService).ToArray();
            var url = $"{_data.HashedAccountId}/apply/{_data.HashedAccountLegalEntityId}/select-apprentices";
            var form = new KeyValuePair<string, string>("SelectedApprenticeships", apprenticeships.First().Id);

            _response = await _testContext.WebsiteClient.PostFormAsync(url, form);
        }

        [Then(@"the employer is asked to supply employment start dates for the selected apprentices")]
        public void ThenTheEmployerIsAskedToSupplyEmploymentStartDatesForTheSelectedApprentices()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().StartWith($"/{_data.HashedAccountId}/apply/");
            _response.RequestMessage.RequestUri.PathAndQuery.Should().EndWith("join-organisation");
        }

        [When(@"the employer supplies valid start dates for the selected apprentices")]
        public async Task WhenTheEmployerSuppliesValidStartDatesForTheSelectedApprentices()
        {
            var employmentStartDatesRequest = new EmploymentStartDatesRequest
            {
                AccountId = _data.HashedAccountId,
                AccountLegalEntityId = _data.HashedAccountLegalEntityId,
                ApplicationId = _data.ApplicationId,
                EmploymentStartDateDays = new List<int?> { 10, 20},
                EmploymentStartDateMonths = new List<int?> { 4, 5 },
                EmploymentStartDateYears = new List<int?> { 2021, 2021 }
            };

            var url = $"{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation";
            
            _response = await _testContext.WebsiteClient.PostAsJsonAsync(url, employmentStartDatesRequest);
        }

        [Then(@"the employer is asked to confirm their selected apprentices")]
        public void ThenTheEmployerIsAskedToConfirmTheirSelectedApprentices()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().StartWith($"/{_data.HashedAccountId}/apply/confirm-apprentices/");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationConfirmationViewModel;
            model.Should().NotBeNull();
            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");
            model.Should().HaveTitle("Confirm apprentices");
        }

        [When(@"the employer supplies invalid start dates for the selected apprentices")]
        public async Task WhenTheEmployerSuppliesInvalidStartDatesForTheSelectedApprentices()
        {
            var employmentStartDatesRequest = new EmploymentStartDatesRequest
            {
                AccountId = _data.HashedAccountId,
                AccountLegalEntityId = _data.HashedAccountLegalEntityId,
                ApplicationId = _data.ApplicationId,
                EmploymentStartDateDays = new List<int?> { 32, 20 },
                EmploymentStartDateMonths = new List<int?> { 4, 13 },
                EmploymentStartDateYears = new List<int?> { 2021, 2021 }
            };

            var url = $"{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation";

            _response = await _testContext.WebsiteClient.PostAsJsonAsync(url, employmentStartDatesRequest);
        }

        [Then(@"the employer is asked to change their submitted dates")]
        public void ThenTheEmployerIsAskedToChangeTheirSubmittedDates()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            model.DateValidationResults.Should().NotBeEmpty();
            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/select-apprentices/{_data.ApplicationId}");
        }

    }
}
