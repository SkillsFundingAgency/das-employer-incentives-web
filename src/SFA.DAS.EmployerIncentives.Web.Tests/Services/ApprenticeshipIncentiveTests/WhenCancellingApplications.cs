using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.ApprenticeshipIncentiveTests
{
    [TestFixture]
    public class WhenCancellingApplications
    {
        private HttpClient _httpClient;
        private FakeHttpMessageHandler _httpClientHandlerFake;
        private Mock<IAccountEncodingService> _accountEncodingServiceMock;
        private HttpResponseMessage _httpResponseMessage;
        private ApprenticeshipIncentiveService _sut;
        private Fixture _fixture;
        private readonly string _baseUrl = "http://www.someurl.com";

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _httpClientHandlerFake = new FakeHttpMessageHandler();

            _accountEncodingServiceMock = new Mock<IAccountEncodingService>();

            _httpClient = new HttpClient(_httpClientHandlerFake)
            {
                BaseAddress = new Uri(_baseUrl)
            };

            _sut = new ApprenticeshipIncentiveService(_httpClient, _accountEncodingServiceMock.Object);
        }

        [Test]
        public async Task Then_the_applications_to_cancel_are_sent_to_the_api_endpoint()
        {
            // Arrange
            var accountId = _fixture.Create<string>();
            var accountLegalEntityId = _fixture.Create<string>();
            var decodedAccountId = _fixture.Create<long>();
            var decodedAccountLegalEntityId = _fixture.Create<long>();
            _accountEncodingServiceMock.Setup(x => x.Decode(accountId)).Returns(decodedAccountId);
            _accountEncodingServiceMock.Setup(x => x.Decode(accountLegalEntityId)).Returns(decodedAccountLegalEntityId);
            
            var emailAddress = _fixture.Create<string>();
            var apprenticeshipIncentives = _fixture.CreateMany<ApprenticeshipIncentiveModel>(3).ToList();

            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);
            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;

            // Act
            await _sut.Cancel(accountLegalEntityId, apprenticeshipIncentives, accountId, emailAddress);

            // Assert
            var uri = new Uri(_baseUrl + "/withdrawals", UriKind.RelativeOrAbsolute);
            _httpClientHandlerFake.LastRequestMessage.Method.Should().Be(HttpMethod.Post);
            _httpClientHandlerFake.LastRequestMessage.RequestUri.Should().Be(uri);
            var json = await _httpClientHandlerFake.LastRequestMessage.Content.ReadAsStringAsync();
            var withdrawRequest = JsonSerializer.Deserialize<WithdrawRequest>(json);

            withdrawRequest.Should().NotBeNull();
            withdrawRequest.AccountId.Should().Be(decodedAccountId);
            withdrawRequest.EmailAddress.Should().Be(emailAddress);
            withdrawRequest.Applications.Should().NotBeNull();
            foreach(var application in apprenticeshipIncentives)
            {
                withdrawRequest.Applications.FirstOrDefault(
                    x => x.AccountLegalEntityId == decodedAccountLegalEntityId && x.ULN == application.Uln)
                    .Should().NotBeNull();
            }
        }
    }
}
