using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.ApprenticeshipIncentiveTests
{
    [TestFixture]
    public class WhenRetrievingApplications
    {
        private HttpClient _httpClient;
        private FakeHttpMessageHandler _httpClientHandlerFake;
        private Mock<IHashingService> _hashingServiceMock;
        private HttpResponseMessage _httpResponseMessage;
        private string _accountId;
        private string _accountLegalEntityId;
        private ApplicationService _sut;
        private Fixture _fixture;
        private string _baseUrl = "http://www.someurl.com";

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _accountId = _fixture.Create<long>().ToString();
            _accountLegalEntityId = _fixture.Create<long>().ToString();

            _httpClientHandlerFake = new FakeHttpMessageHandler();

            _hashingServiceMock = new Mock<IHashingService>();
            _hashingServiceMock.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns((string s) => Convert.ToInt64(s));

            _httpClient = new HttpClient(_httpClientHandlerFake)
            {
                BaseAddress = new Uri(_baseUrl)
            };

            _sut = new ApplicationService(_httpClient, _hashingServiceMock.Object);
        }

        [Test]
        public async Task Then_a_list_of_applications_is_received()
        {
            var applicationList = new List<ApprenticeApplicationModel> 
            { 
                _fixture.Create<ApprenticeApplicationModel>(),
                _fixture.Create<ApprenticeApplicationModel>()
            };
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applicationList, BankDetailsStatus = BankDetailsStatus.InProgress };

            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            { 
                Content = new ObjectContent<GetApplicationsModel>(getApplicationsResponse, new JsonMediaTypeFormatter(), "application/json")
            };

            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;

            var response = await _sut.GetList(_accountId, _accountLegalEntityId);

            response.ApprenticeApplications.Count().Should().Be(2);
        }

        [Test]
        public async Task Then_no_applications_are_received()
        {
            _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            _httpClientHandlerFake.ExpectedResponseMessage = _httpResponseMessage;

            var response = await _sut.GetList(_accountId, _accountLegalEntityId);

            response.ApprenticeApplications.Count().Should().Be(0);
        }
    }
}
