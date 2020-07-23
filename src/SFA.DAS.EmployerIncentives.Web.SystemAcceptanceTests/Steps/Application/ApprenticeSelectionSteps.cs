using AngleSharp.Html.Parser;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
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
    public class ApprenticeSelectionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestDataStore _testData;
        private readonly IHashingService _hashingService;
        private HttpResponseMessage _continueNavigationResponse;
        private List<ApprenticeDto> _apprenticeshipData;

        public ApprenticeSelectionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = _testContext.TestDataStore;
            _hashingService = _testContext.HashingService;
        }

        [Given(@"an employer applying for a grant has apprentices matching the eligibility requirement")]
        public void GivenAnEmployerApplyingForAGrantHasApprenticesMatchingTheEligibilityRequirement()
        {
            var data = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();
            _apprenticeshipData = data.Apprentices;

            var accountId = _testData.GetOrCreate("AccountId", onCreate: () => data.AccountId);
            _testData.Add("HashedAccountId", _hashingService.HashValue(accountId));
            var accountLegalEntityId = _testData.GetOrCreate("AccountLegalEntityId", onCreate: () => data.AccountLegalEntityId);
            _testData.Add("HashedAccountLegalEntityId", _hashingService.HashValue(accountLegalEntityId));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", accountId.ToString())
                        .WithParam("accountlegalentityid", accountLegalEntityId.ToString())
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_apprenticeshipData))
                        .WithStatusCode(HttpStatusCode.OK));

        }

        [When(@"the employer selects the apprentice the grant applies to")]
        public async Task WhenTheEmployerSelectsTheApprenticeTheGrantAppliesTo()
        {
            var apprenticeships = _apprenticeshipData.ToApprenticeshipModel(_hashingService).ToArray();

            var viewModelWithValidSelection = new SelectApprenticeshipsViewModel
            {
                Apprenticeships = apprenticeships,
                SelectedApprenticeships = new List<string> { apprenticeships.First().Id }
            };

            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/apply/{hashedLegalEntityId}/select-new-apprentices";

            _continueNavigationResponse = await _testContext.WebsiteClient.PostValueAsync(url, viewModelWithValidSelection);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [When(@"the employer doesn't select any apprentice the grant applies to")]
        public async Task WhenTheEmployerDoesnTSelectAnyApprenticeTheGrantAppliesTo()
        {
            var apprenticeships = _apprenticeshipData.ToApprenticeshipModel(_hashingService).ToArray();

            var viewModelWithValidSelection = new SelectApprenticeshipsViewModel
            {
                Apprenticeships = apprenticeships,
                SelectedApprenticeships = new List<string>() // no selection made
            };

            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"{hashedAccountId}/apply/{hashedLegalEntityId}/select-new-apprentices";

            _continueNavigationResponse = await _testContext.WebsiteClient.PostValueAsync(url, viewModelWithValidSelection);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked to sign the declaration")]
        public async Task ThenTheEmployerIsAskedToSignTheDeclaration()
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await _continueNavigationResponse.Content.ReadAsStreamAsync());
            var hashedAccountId = _testData.Get<string>("HashedAccountId");

            document.Title.Should().Be("Declaration");
            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{hashedAccountId}/apply/declaration");
        }

        [Then(@"the employer is informed that they haven't selected an apprentice")]
        public async Task ThenTheEmployerIsInformedThatTheyHavenTSelectedAnApprentice()
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await _continueNavigationResponse.Content.ReadAsStreamAsync());

            document.Title.Should().Be(SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);

            var hashedAccountId = _testData.Get<string>("HashedAccountId");
            var hashedLegalEntityId = _testData.Get<string>("HashedAccountLegalEntityId");

            var url = $"/{hashedAccountId}/apply/{hashedLegalEntityId}/select-new-apprentices";

            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be(url);
        }
    }
}
