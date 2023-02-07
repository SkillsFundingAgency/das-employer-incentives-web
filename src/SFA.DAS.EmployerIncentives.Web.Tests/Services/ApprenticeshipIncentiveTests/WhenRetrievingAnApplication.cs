using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.ApprenticeshipIncentiveTests
{
    [TestFixture]
    public class WhenRetrievingAnApplication
    {
        private HttpClient _httpClient;
        private FakeHttpMessageHandler _httpClientHandlerFake;
        private Mock<IAccountEncodingService> _encodingServiceMock;
        private HttpResponseMessage _httpResponseMessage;
        private ApplicationService _sut;
        private Fixture _fixture;
        private readonly string _baseUrl = "http://www.someurl.com";

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _httpClientHandlerFake = new FakeHttpMessageHandler();

            _encodingServiceMock = new Mock<IAccountEncodingService>();
            _encodingServiceMock.Setup(x => x.Decode(It.IsAny<string>())).Returns((string s) => Convert.ToInt64(s));

            _httpClient = new HttpClient(_httpClientHandlerFake)
            {
                BaseAddress = new Uri(_baseUrl)
            };

            _sut = new ApplicationService(_httpClient, _encodingServiceMock.Object);
        }

        [Test]
        public async Task Then_the_submitted_application_is_returned_when_includeSubmitted_is_true()
        {
            var getApplicationsResponse = new ApplicationResponse 
            {
                Application = _fixture.Create<IncentiveApplicationDto>()
            };

            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            { 
                Content = new ObjectContent<ApplicationResponse>(getApplicationsResponse, new JsonMediaTypeFormatter(), "application/json")
            };

            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;

            var response = await _sut.Get(It.IsAny<string>(), It.IsAny<Guid>(), includeSubmitted: true);

            response.Should().NotBeNull();
        }

        [Test]
        public async Task Then_the_submitted_application_is_not_returned_when_includeSubmitted_is_false()
        {
            var getApplicationsResponse = new ApplicationResponse
            {
                Application = _fixture.Create<IncentiveApplicationDto>()
            };

            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<ApplicationResponse>(getApplicationsResponse, new JsonMediaTypeFormatter(), "application/json")
            };

            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;

            var response = await _sut.Get(It.IsAny<string>(), It.IsAny<Guid>());

            response.Should().BeNull();
        }

        [Test]
        public async Task Then_the_active_application_is_returned_when_includeSubmitted_is_false()
        {
            var getApplicationsResponse = new ApplicationResponse
            {
                Application = _fixture.Build<IncentiveApplicationDto>().Without(a => a.SubmittedByEmail).Create()
            };

            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<ApplicationResponse>(getApplicationsResponse, new JsonMediaTypeFormatter(), "application/json")
            };

            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;

            var response = await _sut.Get(It.IsAny<string>(), It.IsAny<Guid>());

            response.Should().NotBeNull();
        }

        [Test]
        public async Task Then_the_active_application_is_returned_when_includeSubmitted_is_true()
        {
            var getApplicationsResponse = new ApplicationResponse
            {
                Application = _fixture.Build<IncentiveApplicationDto>().Without(a => a.SubmittedByEmail).Create()
            };

            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<ApplicationResponse>(getApplicationsResponse, new JsonMediaTypeFormatter(), "application/json")
            };

            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;

            var response = await _sut.Get(It.IsAny<string>(), It.IsAny<Guid>(), includeSubmitted: true);

            response.Should().NotBeNull();
        }
    }
}
