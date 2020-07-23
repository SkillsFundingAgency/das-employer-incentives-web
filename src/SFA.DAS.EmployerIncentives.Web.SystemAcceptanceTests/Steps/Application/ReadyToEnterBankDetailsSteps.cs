using AngleSharp.Html.Parser;
using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    public class ReadyToEnterBankDetailsSteps : StepsBase
    {
        private const string ReadyToEnterBankDetailsUrl = "need-bank-details";
        private const string NeedBankDetailsUrl = "complete/need-bank-details";
        private const string EnterBankDetailsUrl = "enter-bank-details";
        private const string ErrorHeading = "There is a problem";

        private readonly TestContext _testContext;
        private readonly TestDataStore _testData;
        private readonly IHashingService _hashingService;
        private HttpResponseMessage _continueNavigationResponse;

        public ReadyToEnterBankDetailsSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _testData = _testContext.TestDataStore;
            _hashingService = _testContext.HashingService;
        }

        [When(@"the employer has confirmed their apprenticeship details")]
        public async Task WhenTheEmployerHasConfirmedTheirApprenticeshipDetails()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());

            var url = $"{hashedAccountId}/apply/{ReadyToEnterBankDetailsUrl}";

            _continueNavigationResponse = await _testContext.WebsiteClient.GetAsync(url);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is asked whether they can provide their organisation's bank details now")]
        public async Task ThenTheEmployerIsAskedWhetherTheyCanProvideTheirOrganisationSBankDetailsNow()
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await _continueNavigationResponse.Content.ReadAsStreamAsync());

            document.Title.Should().Be(new BankDetailsConfirmationViewModel().Title);
        }

        [When(@"the employer confirms they can provide their bank details")]
        public async Task WhenTheEmployerConfirmsTheyCanProvideTheirBankDetails()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());

            var request = new HttpRequestMessage(
             HttpMethod.Post,
             $"{hashedAccountId}/apply/{ReadyToEnterBankDetailsUrl}")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("canProvideBankDetails", "true")
                    })
            };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is redirected to the enter bank details page")]
        public void ThenTheEmployerIsRedirectedToTheEnterBankDetailsPage()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());
            var url = $"/{hashedAccountId}/apply/{EnterBankDetailsUrl}";

            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be(url);
        }

        [When(@"the employer states that they are unable to provide bank details now")]
        public async Task WhenTheEmployerStatesThatTheyAreUnableToProvideBankDetailsNow()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());

            var request = new HttpRequestMessage(
               HttpMethod.Post,
               $"{hashedAccountId}/apply/{ReadyToEnterBankDetailsUrl}")
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("canProvideBankDetails", "false")
                    })
                };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is redirected to the ready to enter bank details page")]
        public void ThenTheEmployerIsRedirectedToTheReadyToEnterBankDetailsPage()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());
            var url = $"/{hashedAccountId}/apply/{NeedBankDetailsUrl}";

            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be(url);
        }

        [When(@"the employer does not confirm whether they can provide bank details now")]
        public async Task WhenTheEmployerDoesNotConfirmWhetherTheyCanProvideBankDetailsNow()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());

            var request = new HttpRequestMessage(
               HttpMethod.Post,
               $"{hashedAccountId}/apply/{ReadyToEnterBankDetailsUrl}")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                })
            };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is prompted to confirm with an answer")]
        public async Task ThenTheEmployerIsPromptedToConfirmWithAnAnswer()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());
            var url = $"/{hashedAccountId}/apply/{ReadyToEnterBankDetailsUrl}";

            _continueNavigationResponse.RequestMessage.RequestUri.LocalPath.Should().Be(url);

            var parser = new HtmlParser();
            var document = parser.ParseDocument(await _continueNavigationResponse.Content.ReadAsStreamAsync());

            document.Title.Should().Be(new BankDetailsConfirmationViewModel().Title);
            document.DocumentElement.InnerHtml.Should().Contain(ErrorHeading);
            document.DocumentElement.InnerHtml.Should().Contain(BankDetailsConfirmationViewModel.CanProvideBankDetailsNotSelectedMessage);
        }

    }
}
