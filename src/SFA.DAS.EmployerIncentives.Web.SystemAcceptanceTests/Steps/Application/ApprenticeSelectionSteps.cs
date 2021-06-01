using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "ApprenticeSelection")]
    public class ApprenticeSelectionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testData;
        private readonly IHashingService _hashingService;
        private HttpResponseMessage _continueNavigationResponse;
        private EligibleApprenticesDto _apprenticeshipData;
        private LegalEntityDto _legalEntity;

        public ApprenticeSelectionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = _testContext.TestDataStore;
            _hashingService = _testContext.HashingService;
        }

        [Given(@"an employer applying for a grant has apprentices matching the eligibility requirement")]
        public void GivenAnEmployerApplyingForAGrantHasApprenticesMatchingTheEligibilityRequirement()
        {
            var data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _apprenticeshipData = data.Apprentices;
            _legalEntity = data.LegalEntities.First();

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => data.AccountId);
            _testData.Add("HashedAccountId", _hashingService.HashValue(accountId));
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _hashingService.HashValue(accountId));
            var accountLegalEntityId = _testData.GetOrCreate("AccountLegalEntityId", onCreate: () => data.AccountLegalEntityId);
            _testData.Add("HashedAccountLegalEntityId", _hashingService.HashValue(accountLegalEntityId));
            
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", accountId.ToString())
                        .WithParam("accountlegalentityid", accountLegalEntityId.ToString())
                        .WithParam("pageNumber", "1")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_apprenticeshipData, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{accountId}/applications")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Created));
            
            _testContext.EmployerIncentivesApi.MockServer
               .Given(
                   Request
                       .Create()
                       .WithPath($"/accounts/{accountId}/applications")
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
                      x.Contains($"accounts/{data.AccountId}/applications") &&
                      x.Contains("accountlegalentity")) // applicationid is generated in application service so will vary per request
                      .UsingGet()
              )
              .RespondWith(
                  Response.Create()
                      .WithStatusCode(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json")
                      .WithBody(data.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{_legalEntity.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_legalEntity))
                        .WithStatusCode(HttpStatusCode.OK));
        }

        [When(@"the employer selects the apprentice the grant applies to")]
        public async Task WhenTheEmployerSelectsTheApprenticeTheGrantAppliesTo()
        {
            var data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"/accounts/{data.AccountId}/applications/") && !x.Contains("accountlegalentity"))
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonConvert.SerializeObject(data.ApplicationResponse, TestHelper.DefaultSerialiserSettings)));

            var apprenticeships = _apprenticeshipData.Apprenticeships.ToApprenticeshipModel(_hashingService).ToArray();
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/apply/{hashedLegalEntityId}/complete-apprentices";
            var form = new KeyValuePair<string, string>("SelectedApprenticeships", apprenticeships.First().Id);

            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url, form);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"the employer doesn't select any apprentice the grant applies to")]
        public async Task WhenTheEmployerDoesnTSelectAnyApprenticeTheGrantAppliesTo()
        {
            var data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"/accounts/{data.AccountId}/applications/") && !x.Contains("accountlegalentity"))
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonConvert.SerializeObject(data.EmptyApplicationResponse, TestHelper.DefaultSerialiserSettings)));

            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/apply/{hashedLegalEntityId}/complete-apprentices";

            _continueNavigationResponse = await _testContext.WebsiteClient.PostFormAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked to provide employment start dates for the apprentices")]
        public void ThenTheEmployerIsAskedToProvideStartDatesForTheSelectedApprentices()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/{model.ApplicationId}/join-organisation");
            _continueNavigationResponse.Should().HaveBackLink($"/{hashedAccountId}/apply/select-apprentices/{model.ApplicationId}");
            model.Should().HaveTitle($"When did they join {_legalEntity.LegalEntityName}?");
        }

        [Then(@"the employer is informed that they haven't selected an apprentice")]
        public void ThenTheEmployerIsInformedThatTheyHaventSelectedAnApprentice()
        {
            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");
            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();
            _continueNavigationResponse.Should().HaveTitle(model.Title);
            _continueNavigationResponse.Should().HavePathAndQuery($"/{hashedAccountId}/apply/{hashedLegalEntityId}/complete-apprentices");
            model.Should().HaveTitle("Which apprentices do you want to apply for?");
            model.Apprenticeships.Count().Should().Be(0);
            model.AccountId.Should().Be(hashedAccountId);
            viewResult.Should().ContainError(model.FirstCheckboxId, SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
        }

        [Given(@"an employer applying for a grant has more apprentices than can be displayed on one page")]
        public async Task GivenAnEmployerApplyingForAGrantHasMoreApprenticesThanCanBeDisplayedOnOnePage()
        {
            var fixture = new Fixture();
            var apprenticeshipData1 = new EligibleApprenticesDto
            {
                PageNumber = 1,
                PageSize = 50,
                TotalApprenticeships = 105,
                Apprenticeships = fixture.CreateMany<ApprenticeDto>(50).ToList()
            };
            var apprenticeshipData2 = new EligibleApprenticesDto
            {
                PageNumber = 2,
                PageSize = 50,
                TotalApprenticeships = 105,
                Apprenticeships = fixture.CreateMany<ApprenticeDto>(50).ToList()
            };
            var apprenticeshipData3 = new EligibleApprenticesDto
            {
                PageNumber = 3,
                PageSize = 50,
                TotalApprenticeships = 105,
                Apprenticeships = fixture.CreateMany<ApprenticeDto>(5).ToList()
            };

            var data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _hashingService.HashValue(data.AccountId));
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", data.AccountId.ToString())
                        .WithParam("accountlegalentityid", data.AccountLegalEntityId.ToString())
                        .WithParam("pageNumber", "1")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(apprenticeshipData1, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", data.AccountId.ToString())
                        .WithParam("accountlegalentityid", data.AccountLegalEntityId.ToString())
                        .WithParam("pageNumber", "2")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(apprenticeshipData2, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", data.AccountId.ToString())
                        .WithParam("accountlegalentityid", data.AccountLegalEntityId.ToString())
                        .WithParam("pageNumber", "3")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(apprenticeshipData3, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            var url = $"{data.HashedAccountId}/apply/{data.HashedAccountLegalEntityId}/select-apprentices?pageNumber=1";

            _continueNavigationResponse = await _testContext.WebsiteClient.GetAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is offered the choice of viewing more apprentices")]
        public async Task ThenTheEmployerIsOfferedTheChoiceOfViewingMoreApprentices()
        {
            _continueNavigationResponse.Should().HaveButton("Next page of apprentices");
        }

        [When(@"the employer has viewed more apprentices")]
        public async Task WhenTheEmployerHasViewedMoreApprentices()
        {
            var data = new TestData.Account.WithInitialApplicationForASingleEntity();
            var url = $"{data.HashedAccountId}/apply/{data.HashedAccountLegalEntityId}/select-apprentices?pageNumber=2&pageSize=50&startIndex=51";

            _continueNavigationResponse = await _testContext.WebsiteClient.GetAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"there are no more apprentices to show")]
        public async Task WhenThereAreNoMoreApprenticesToShow()
        {
            var fixture = new Fixture();
            var apprenticeshipData2 = new EligibleApprenticesDto
            {
                PageNumber = 2,
                PageSize = 50,
                TotalApprenticeships = 80,
                Apprenticeships = fixture.CreateMany<ApprenticeDto>(30).ToList()
            };

            var data = new TestData.Account.WithInitialApplicationForASingleEntity();
            
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", data.AccountId.ToString())
                        .WithParam("accountlegalentityid", data.AccountLegalEntityId.ToString())
                        .WithParam("pageNumber", "2")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(apprenticeshipData2, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));
            
        }

        [Then(@"the employer is offered the choice of viewing previous apprentices")]
        public void ThenTheEmployerIsOfferedTheChoiceOfViewingPreviousApprentices()
        {
            _continueNavigationResponse.Should().HaveButton("Previous page of apprentices");
        }

        [When(@"there are more apprentices to show")]
        public void WhenThereAreMoreApprenticesToShow()
        {
            var fixture = new Fixture();
            var apprenticeshipData2 = new EligibleApprenticesDto
            {
                PageNumber = 2,
                PageSize = 50,
                TotalApprenticeships = 110,
                Apprenticeships = fixture.CreateMany<ApprenticeDto>(50).ToList()
            };

            var data = new TestData.Account.WithInitialApplicationForASingleEntity();

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", data.AccountId.ToString())
                        .WithParam("accountlegalentityid", data.AccountLegalEntityId.ToString())
                        .WithParam("pageNumber", "2")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(apprenticeshipData2, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

        }

    }
}
