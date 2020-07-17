using AngleSharp.Html.Parser;
using FluentAssertions;
using Newtonsoft.Json;
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

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "ApplicationEligibility")]
    public class ApplicationEligibilitySteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly TestData _testData;
        private readonly IHashingService _hashingService;

        public ApplicationEligibilitySteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = _testContext.TestData;
            _hashingService = _testContext.HashingService;
        }

        [Given(@"an employer applying for a grant has no apprentices matching the eligibility requirement")]
        public void GivenAnEmployerApplyingHasNoApprenticesMatchingTheEligibilityCriteria()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            _testData.Add("hashedAccountId", _hashingService.HashValue(accountId));
            var accountLegalEntityId =_testData.GetOrCreate<long>("AccountLegalEntityId");

            var legalEntity = _testContext.TestData.GetOrCreate("LegalEntityDto", onCreate: () =>
            {
                return new LegalEntityDto
                {
                    AccountId = accountId,
                    AccountLegalEntityId = accountLegalEntityId,
                    LegalEntityName = _testData.GetOrCreate<string>("LegalEntityName")
                };
            });

            _testContext.EmployerIncentivesApi.MockServer
              .Given(
                      Request
                      .Create()
                      .WithPath($"/accounts/{accountId}/legalentities")
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithStatusCode(HttpStatusCode.OK)                  
                  .WithBody(JsonConvert.SerializeObject(new List<LegalEntityDto>() { legalEntity })));

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
                  .WithStatusCode(HttpStatusCode.OK));
        }

        [When(@"the employer tries to make a grant application")]
        public async Task WhenTheEmployerMakesAGrantApplication()
        {
            var viewModel = new QualificationQuestionViewModel
            {
                HasTakenOnNewApprentices = true
            };

            var response = await _testContext.WebsiteClient.PostAsJsonAsync($"{_testData.Get<string>("hashedAccountId")}/apply", viewModel);
            _testContext.TestData.GetOrCreate("ApplicationEligibilityResponse", onCreate: () =>
            {
                return response;
            });
        }

        [Then(@"the employer is informed they cannot apply for the grant")]
        public async Task ThenTheEmployerIsInformedTheycannotApplyForTheGrant()
        {
            var response = _testData.Get<HttpResponseMessage>("ApplicationEligibilityResponse");
            response.EnsureSuccessStatusCode();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            document.Title.Should().Be("You cannot apply for this grant");

            var allRequests = _testContext.EmployerIncentivesApi.MockServer.FindLogEntries();
            var accountId = _testData.Get<long>("AccountId");
            var accountLegalEntityId = _testData.Get<long>("AccountLegalEntityId");

            var requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath($"/accounts/{accountId}/legalentities")
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);

            requests = _testContext
                       .EmployerIncentivesApi
                       .MockServer
                       .FindLogEntries(
                           Request
                           .Create()
                           .WithPath("/apprenticeships")
                           .WithParam("accountid", accountId.ToString())
                           .WithParam("accountlegalentityid", accountLegalEntityId.ToString())
                           .UsingGet());

            requests.AsEnumerable().Count().Should().Be(1);
        }
    }
}
