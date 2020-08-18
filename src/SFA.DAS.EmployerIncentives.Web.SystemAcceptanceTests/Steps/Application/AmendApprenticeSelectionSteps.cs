using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using SFA.DAS.HashingService;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    public class AmendApprenticeSelectionSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly IHashingService _hashingService;
        private List<ApprenticeDto> _apprenticeshipData;
        private TestData.Account.WithInitialApplicationForASingleEntity _data;
        private HttpResponseMessage _response;

        public AmendApprenticeSelectionSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _hashingService = _testContext.HashingService;
            _data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _data.HashedAccountId);
        }

        [Given(@"there are eligible apprenticeships for the grant")]
        public void GivenThereAreEligibleApprenticeshipsForTheGrant()
        {
            _apprenticeshipData = _data.Apprentices;

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/apprenticeships")
                        .WithParam("accountid", _data.AccountId.ToString())
                        .WithParam("accountlegalentityid", _data.AccountLegalEntityId.ToString())
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_apprenticeshipData, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));
        }

        [Given(@"a initial application has been created")]
        public void GivenAInitialApplicationHasBeenCreated()
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
        }

        [Given(@"a initial application has been created and it includes an apprentice who is no longer eligible")]
        public void GivenAInitialApplicationHasBeenCreatedAndItIncludesAnApprenticeWhoIsNoLongerEligible()
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
                        .WithBody(JsonConvert.SerializeObject(_data.GetApplicationResponseWithFirstTwoApprenticesSelectedAndAnAdditionalApprentice, TestHelper.DefaultSerialiserSettings)));
        }

        [When(@"the employer returns to the select apprentices page")]
        public async Task WhenTheEmployerReturnsToTheSelectApprenticesPage()
        {
            var hashedAccountId = _hashingService.HashValue(_data.AccountId);

            var url = $"{hashedAccountId}/apply/select-apprentices/{_data.ApplicationId}";

            _response = await _testContext.WebsiteClient.GetAsync(url);
            _response.EnsureSuccessStatusCode();
        }

        [Then(@"the employer can see the previous apprentices checked")]
        public void ThenTheEmployerCanSeeThePreviousApprenticesChecked()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;
            model.Should().NotBeNull();

            model.Apprenticeships.Count().Should().Be(3);

            model.Apprenticeships.First(x => x.Id == _hashingService.HashValue(1)).Selected.Should().BeTrue();
            model.Apprenticeships.First(x => x.Id == _hashingService.HashValue(2)).Selected.Should().BeTrue();
            model.Apprenticeships.First(x => x.Id == _hashingService.HashValue(3)).Selected.Should().BeFalse();
        }

        [Then(@"the additional apprentice is not on the list")]
        public void ThenTheAdditionalApprenticeIsNotOnTheList()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SelectApprenticeshipsViewModel;

            model.Apprenticeships.FirstOrDefault(x => x.Id == _hashingService.HashValue(99)).Should().BeNull();
        }
    }
}
