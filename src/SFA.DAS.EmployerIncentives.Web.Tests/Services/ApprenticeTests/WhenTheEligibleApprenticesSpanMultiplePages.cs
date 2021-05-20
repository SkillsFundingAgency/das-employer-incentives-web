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
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.ApprenticeTests
{
    [TestFixture]
    public class WhenTheEligibleApprenticesSpanMultiplePages
    {
        private Fixture _fixture;
        private HttpClient _httpClient;
        private QueuedFakeHttpMessageHandler _httpClientHandlerFake;
        private Mock<IHashingService> _hashingService;
        private Mock<IPaginationService> _paginationService;
        private string _baseUrl = "http://www.someurl.com";
        private ApprenticesService _sut;
        private string _accountId;
        private string _accountLegalEntityId;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _httpClientHandlerFake = new QueuedFakeHttpMessageHandler();

            _httpClient = new HttpClient(_httpClientHandlerFake);
            _httpClient.BaseAddress = new Uri(_baseUrl);

            _hashingService = new Mock<IHashingService>();
            _paginationService = new Mock<IPaginationService>();
            _sut = new ApprenticesService(_httpClient, _hashingService.Object, _paginationService.Object);

            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_the_apprenticeships_are_requested_until_the_page_is_populated()
        {
            // Arrange
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 1);

            var apprenticeships1 = _fixture.CreateMany<ApprenticeDto>(22).ToList();
            var eligibleApprenticeships1 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships1, PageNumber = 1, PageSize = 50, TotalApprenticeships = 49 };

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var apprenticeships2 = _fixture.CreateMany<ApprenticeDto>(16).ToList();
            var eligibleApprenticeships2 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships2, PageNumber = 2, PageSize = 50, TotalApprenticeships = 49 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            var eligibleApprenticeships3 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 3, PageSize = 50, TotalApprenticeships = 49 };

            var httpResponseMessage3 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships3,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage3);

            // Act
            var response = await _sut.Get(query);

            // Assert
            response.Apprenticeships.Count().Should().Be(apprenticeships1.Count + apprenticeships2.Count);
            response.MorePages.Should().BeFalse();
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(2);
        }

        [Test]
        public async Task Then_the_response_indicates_that_there_are_more_pages_of_apprenticeships()
        {
            // Arrange
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 1);

            var apprenticeships1 = _fixture.CreateMany<ApprenticeDto>(34).ToList();
            var eligibleApprenticeships1 = new EligibleApprenticesDto
            { Apprenticeships = apprenticeships1, PageNumber = 1, PageSize = 50, TotalApprenticeships = 58 };

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var apprenticeships2 = _fixture.CreateMany<ApprenticeDto>(23).ToList();
            var eligibleApprenticeships2 = new EligibleApprenticesDto
            { Apprenticeships = apprenticeships2, PageNumber = 2, PageSize = 50, TotalApprenticeships = 58 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            // Act
            var response = await _sut.Get(query);

            // Assert
            response.Apprenticeships.Count().Should().Be(query.PageSize);
            response.MorePages.Should().BeTrue();
            response.Offset.Should().Be(16);
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(2);
        }

        [Test]
        public async Task Then_the_next_page_starts_with_an_offset_if_the_previous_page_was_partially_returned()
        {
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 20, startIndex: 51);

            var apprenticeships1 = _fixture.CreateMany<ApprenticeDto>(46).ToList();
            var eligibleApprenticeships1 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships1, PageNumber = 2, PageSize = 50, TotalApprenticeships = 66 };

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var eligibleApprenticeships2 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 3, PageSize = 50, TotalApprenticeships = 66 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            // Act
            var response = await _sut.Get(query);

            // Assert
            response.Apprenticeships.Count().Should().Be(apprenticeships1.Count - query.Offset);
            response.MorePages.Should().BeFalse();
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(1);
        }
    }
}
