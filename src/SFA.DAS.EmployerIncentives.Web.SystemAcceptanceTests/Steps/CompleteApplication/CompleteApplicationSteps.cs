using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.CompleteApplication
{
    [Binding]
    [Scope(Feature = "CompleteApplication")]
    public class CompleteApplicationSteps : StepsBase
    {
        private readonly TestContext _testContext;

        public CompleteApplicationSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
        }

        [Given(@"the employer has entered all the information required to process their bank details")]
        public void GivenTheEmployerHasEnteredAllTheInformationRequiredToProcessTheirBankDetails()
        {
            // TODO: will require call back information to be passed back from Business Central
        }

        [When(@"the employer is shown the confirmation page")]
        public async Task WhenTheEmployerIsShownTheConfirmationPage()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/application-complete");

            var response = await _testContext.WebsiteClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"the employer has the option to return to their accounts page")]
        public void ThenTheEmployerHasTheOptionToReturnToTheirAccountsPage()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmationViewModel;
            model.Should().NotBeNull();
            model.AccountsUrl.Should().NotBeNullOrEmpty();
        }

    }
}
