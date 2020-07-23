using AngleSharp.Html.Parser;
using FluentAssertions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "ReadyToEnterBankDetails")]
    public class ReadyToEnterBankDetailsSteps : StepsBase
    {
        private const string ReadyToEnterBankDetailsUrl = "need-bank-details";
        private const string NeedBankDetailsUrl = "complete/need-bank-details";
        private const string EnterBankDetailsUrl = "enter-bank-details";

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
            var legalEntityId = _testData.GetOrCreate<long>("LegalEntityId");
            var hashedLegalEntityId = _hashingService.HashValue(legalEntityId.ToString());

            var url = $"{hashedAccountId}/apply/{hashedLegalEntityId}/{ReadyToEnterBankDetailsUrl}";

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
            var legalEntityId = _testData.GetOrCreate<long>("LegalEntityId");
            var hashedLegalEntityId = _hashingService.HashValue(legalEntityId.ToString());

            var request = new HttpRequestMessage(
             HttpMethod.Post,
             $"{hashedAccountId}/apply/{hashedLegalEntityId}/{ReadyToEnterBankDetailsUrl}")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("canProvideBankDetails", "true")
                    })
            };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is requested to enter their bank details")]
        public void ThenTheEmployerIsRedirectedToTheEnterBankDetailsPage()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());
            var legalEntityId = _testData.GetOrCreate<long>("LegalEntityId");
            var hashedLegalEntityId = _hashingService.HashValue(legalEntityId.ToString());

            var url = $"/{hashedAccountId}/apply/{hashedLegalEntityId}/{EnterBankDetailsUrl}";

            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be(url);
        }

        [When(@"the employer states that they are unable to provide bank details now")]
        public async Task WhenTheEmployerStatesThatTheyAreUnableToProvideBankDetailsNow()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());
            var legalEntityId = _testData.GetOrCreate<long>("LegalEntityId");
            var hashedLegalEntityId = _hashingService.HashValue(legalEntityId.ToString());

            var request = new HttpRequestMessage(
               HttpMethod.Post,
               $"{hashedAccountId}/apply/{hashedLegalEntityId}/{ReadyToEnterBankDetailsUrl}")
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("canProvideBankDetails", "false")
                    })
                };

            _continueNavigationResponse = await _testContext.WebsiteClient.SendAsync(request);
            _continueNavigationResponse.EnsureSuccessStatusCode();
        }

        [Then(@"the employer is requested to enter their bank details at a later date")]
        public void ThenTheEmployerIsRedirectedToTheReadyToEnterBankDetailsPage()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());
            var legalEntityId = _testData.GetOrCreate<long>("LegalEntityId");
            var hashedLegalEntityId = _hashingService.HashValue(legalEntityId.ToString());

            var url = $"/{hashedAccountId}/apply/{hashedLegalEntityId}/{NeedBankDetailsUrl}";

            _continueNavigationResponse.RequestMessage.RequestUri.PathAndQuery.Should().Be(url);
        }

        [When(@"the employer does not confirm whether they can provide bank details now")]
        public async Task WhenTheEmployerDoesNotConfirmWhetherTheyCanProvideBankDetailsNow()
        {
            var accountId = _testData.GetOrCreate<long>("AccountId");
            var hashedAccountId = _hashingService.HashValue(accountId.ToString());
            var legalEntityId = _testData.GetOrCreate<long>("LegalEntityId");
            var hashedLegalEntityId = _hashingService.HashValue(legalEntityId.ToString());

            var request = new HttpRequestMessage(
               HttpMethod.Post,
               $"{hashedAccountId}/apply/{hashedLegalEntityId}/{ReadyToEnterBankDetailsUrl}")
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
            var legalEntityId = _testData.GetOrCreate<long>("LegalEntityId");
            var hashedLegalEntityId = _hashingService.HashValue(legalEntityId.ToString());

            var url = $"/{hashedAccountId}/apply/{hashedLegalEntityId}/{ReadyToEnterBankDetailsUrl}";

            _continueNavigationResponse.RequestMessage.RequestUri.LocalPath.Should().Be(url);

            var parser = new HtmlParser();
            var document = parser.ParseDocument(await _continueNavigationResponse.Content.ReadAsStreamAsync());
            document.Title.Should().Be(new BankDetailsConfirmationViewModel().Title);

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as BankDetailsConfirmationViewModel;
            model.Should().NotBeNull();
            model.CanProvideBankDetails.Should().BeNull();
            viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            viewResult.ViewData.ModelState.ContainsKey(nameof(BankDetailsConfirmationViewModel.CanProvideBankDetails)).Should().BeTrue();
            viewResult.ViewData.ModelState[nameof(BankDetailsConfirmationViewModel.CanProvideBankDetails)]
                .Errors.First().ErrorMessage.Should().Be(BankDetailsConfirmationViewModel.CanProvideBankDetailsNotSelectedMessage);
        }

    }
}
