using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.ApplicationTests
{
    [TestFixture]
    public class WhenUpdatingAnApplication
    {

        private HttpClient _httpClient;
        private FakeHttpMessageHandler _httpClientHandlerFake;
        private Mock<IHashingService> _hashingServiceMock;
        private HttpResponseMessage _httpResponseMessage;
        private Guid _applicationId;
        private string _accountHashedId;
        private long _accountId;
        private long[] _apprenticeshipIds;
        private string[] _apprenticeshipHashedIds;
        private ApplicationService _sut;
        private Fixture _fixture;
        private string _baseUrl = "http://www.someurl.com";

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _applicationId = Guid.NewGuid();
            _accountId = _fixture.Create<long>();
            _accountHashedId = _accountId.ToString();
            _apprenticeshipIds = _fixture.CreateMany<long>().ToArray();
            _apprenticeshipHashedIds = _apprenticeshipIds.Select(x => x.ToString()).ToArray();

            _httpClientHandlerFake = new FakeHttpMessageHandler();
            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);

            _hashingServiceMock = new Mock<IHashingService>();
            _hashingServiceMock.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns((string s) => Convert.ToInt64(s));

            _httpClient = new HttpClient(_httpClientHandlerFake);
            _httpClient.BaseAddress = new Uri(_baseUrl);

            _sut = new ApplicationService(_httpClient, _hashingServiceMock.Object);
        }

        [Test]
        public async Task Then_The_Request_Is_Sent_To_The_Correct_End_Point()
        {
            SetupFakeResponse(HttpStatusCode.Accepted);
            
            await _sut.Update(_applicationId, _accountHashedId, _apprenticeshipHashedIds);

            VerifyEndpointForApplicationUpdate($"/accounts/{_accountId}/applications");
        }

        [Test]
        public void Then_The_Request_Throws_An_Exception_If_It_Gets_None_200_Response()
        {
            SetupFakeResponse(HttpStatusCode.Forbidden);

            Assert.ThrowsAsync<HttpRequestException>(() => _sut.Update(_applicationId, _accountHashedId, _apprenticeshipHashedIds));
        }

        [Test]
        public async Task Then_The_Request_Contains_The_Correctly_Mapped_Body()
        {
            SetupFakeResponse(HttpStatusCode.Accepted);

            await _sut.Update(_applicationId, _accountHashedId, _apprenticeshipHashedIds);

            var body = await _httpClientHandlerFake.LastRequestMessage.Content.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<UpdateApplicationRequest>(body, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

            request.Should().NotBeNull();
            request.AccountId.Should().Be(_accountId);
            request.ApprenticeshipIds.Should().BeEquivalentTo(_apprenticeshipIds);
        }

        private void SetupFakeResponse(HttpStatusCode statusCode)
        {
            _httpResponseMessage.StatusCode = statusCode;
            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;
        }

        private void VerifyEndpointForApplicationUpdate(string path)
        {
            var uri = new Uri(_baseUrl + path, UriKind.RelativeOrAbsolute);
            _httpClientHandlerFake.LastRequestMessage.Method.Should().Be(HttpMethod.Put);
            _httpClientHandlerFake.LastRequestMessage.RequestUri.Should().Be(uri);
        }
    }
}
