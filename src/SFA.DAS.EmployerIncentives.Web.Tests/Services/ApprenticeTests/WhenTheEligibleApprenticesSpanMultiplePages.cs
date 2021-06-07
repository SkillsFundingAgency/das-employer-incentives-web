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
            var query1 = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 1);
            var pagination1 = new PagingInformation 
            {
                Offset = 0,
                PageNumber = 1,
                StartIndex = 1
            };
            _paginationService.Setup(x => x.GetPagingInformation(query1)).ReturnsAsync(pagination1);

            var apprenticeships1 = _fixture.CreateMany<ApprenticeDto>(22).ToList();
            var eligibleApprenticeships1 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships1, PageNumber = 1, PageSize = 50, TotalApprenticeships = 49 };

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var query2 = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 51);
            var pagination2 = new PagingInformation
            {
                Offset = 1,
                PageNumber = 2,
                StartIndex = 51
            };
            _paginationService.Setup(x => x.GetPagingInformation(query2)).ReturnsAsync(pagination2);

            var apprenticeships2 = _fixture.CreateMany<ApprenticeDto>(16).ToList();
            var eligibleApprenticeships2 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships2, PageNumber = 2, PageSize = 50, TotalApprenticeships = 49 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            var query3 = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 101);
            var pagination3 = new PagingInformation
            {
                Offset = 0,
                PageNumber = 3,
                StartIndex = 101
            };
            _paginationService.Setup(x => x.GetPagingInformation(query3)).ReturnsAsync(pagination3);

            var eligibleApprenticeships3 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 3, PageSize = 50, TotalApprenticeships = 49 };

            var httpResponseMessage3 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships3,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage3);

            // Act
            var response = await _sut.Get(query1);

            // Assert
            response.Apprenticeships.Count().Should().Be(apprenticeships1.Count + apprenticeships2.Count);
            response.MorePages.Should().BeFalse();
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(2);
            _paginationService.Verify(x => x.GetPagingInformation(It.Is<ApprenticesQuery>(y => y.StartIndex == 1)), Times.Once);
            _paginationService.Verify(x => x.SavePagingInformation(It.Is<PagingInformation>(y => y.StartIndex == 51)), Times.Once);
        }

        [Test]
        public async Task Then_the_response_indicates_that_there_are_more_pages_of_apprenticeships()
        {
            // Arrange
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 1);
            var pagination = new PagingInformation
            {
                Offset = 0,
                PageNumber = 1,
                StartIndex = 1
            };
            _paginationService.Setup(x => x.GetPagingInformation(query)).ReturnsAsync(pagination);

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
            var pagination = new PagingInformation
            {
                Offset = 20,
                PageNumber = 2,
                StartIndex = 51
            };
            _paginationService.Setup(x => x.GetPagingInformation(query)).ReturnsAsync(pagination);

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

        [Test]
        public async Task Then_the_next_page_is_requested_if_there_are_more_apprentices_and_the_first_page_is_empty()
        {
            // Arrange
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 1);
            var pagination = new PagingInformation
            {
                Offset = 0,
                PageNumber = 1,
                StartIndex = 1
            };
            _paginationService.Setup(x => x.GetPagingInformation(query)).ReturnsAsync(pagination);

            var eligibleApprenticeships1 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 1, PageSize = 50, TotalApprenticeships = 52 };

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var apprenticeships2 = _fixture.CreateMany<ApprenticeDto>(2).ToList();
            var eligibleApprenticeships2 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships2, PageNumber = 2, PageSize = 50, TotalApprenticeships = 52 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            // Act
            var response = await _sut.Get(query);

            // Assert
            response.Apprenticeships.Count().Should().Be(apprenticeships2.Count);
            response.MorePages.Should().BeFalse();
            response.Offset.Should().Be(2);
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(2);
        }

        [Test]
        public async Task Then_no_apprentices_are_returned_if_the_first_page_is_blank_and_there_are_no_more_apprentices()
        {
            // Arrange
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 50, offset: 0, startIndex: 1);
            var pagination = new PagingInformation
            {
                Offset = 0,
                PageNumber = 1,
                StartIndex = 1
            };
            _paginationService.Setup(x => x.GetPagingInformation(query)).ReturnsAsync(pagination);

            var eligibleApprenticeships1 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 1, PageSize = 50, TotalApprenticeships = 50 };

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var eligibleApprenticeships2 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 2, PageSize = 50, TotalApprenticeships = 50 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            // Act
            var response = await _sut.Get(query);

            // Assert
            response.Apprenticeships.Count().Should().Be(0);
            response.MorePages.Should().BeFalse();
            response.Offset.Should().Be(0);
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(2);
        }

        [Test]
        public async Task Then_more_pages_continue_to_be_requested_when_some_are_blank_or_have_excluded_apprenticeships()
        {
            // Arrange
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageSize: 200, offset: 0, startIndex: 1);
            var pagination = new PagingInformation
            {
                Offset = 0,
                PageNumber = 1,
                StartIndex = 1
            };
            _paginationService.Setup(x => x.GetPagingInformation(query)).ReturnsAsync(pagination);

            var apprenticeships1 = _fixture.CreateMany<ApprenticeDto>(5).ToList();
            var eligibleApprenticeships1 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships1, PageNumber = 1, PageSize = 200, TotalApprenticeships = 987 };

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var eligibleApprenticeships2 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 2, PageSize = 200, TotalApprenticeships = 987 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            var apprenticeships3 = _fixture.CreateMany<ApprenticeDto>(111).ToList();
            var eligibleApprenticeships3 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships3, PageNumber = 3, PageSize = 200, TotalApprenticeships = 987 };

            var httpResponseMessage3 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships3,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage3);

            var apprenticeships4 = _fixture.CreateMany<ApprenticeDto>(4).ToList();
            var eligibleApprenticeships4 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships4, PageNumber = 4, PageSize = 200, TotalApprenticeships = 987 };

            var httpResponseMessage4 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships4,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage4);

            var apprenticeships5 = _fixture.CreateMany<ApprenticeDto>(40).ToList();
            var eligibleApprenticeships5 = new EligibleApprenticesDto
                { Apprenticeships = apprenticeships5, PageNumber = 5, PageSize = 200, TotalApprenticeships = 987 };

            var httpResponseMessage5 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships5,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage5);

            var eligibleApprenticeships6 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 6, PageSize = 200, TotalApprenticeships = 987 };

            var httpResponseMessage6 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships6,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage6);

            // Act
            var response = await _sut.Get(query);

            // Assert
            response.Apprenticeships.Count().Should().Be(apprenticeships1.Count + apprenticeships3.Count + apprenticeships4.Count + apprenticeships5.Count);
            response.MorePages.Should().BeFalse();
            response.Offset.Should().Be(0);
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(6);
        }
    }
}
