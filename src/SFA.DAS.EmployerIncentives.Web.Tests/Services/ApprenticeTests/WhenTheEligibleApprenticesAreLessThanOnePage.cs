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
    public class WhenTheEligibleApprenticesAreLessThanOnePage
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
        public async Task Then_the_apprenticeships_are_retrieved_in_a_single_request()
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

            var apprenticeships = _fixture.CreateMany<ApprenticeDto>(49).ToList();
            var eligibleApprenticeships1 = new EligibleApprenticesDto
                {Apprenticeships = apprenticeships, PageNumber = 1, PageSize = 50, TotalApprenticeships = 49};

            var httpResponseMessage1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships1,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage1);

            var eligibleApprenticeships2 = new EligibleApprenticesDto
                { Apprenticeships = new List<ApprenticeDto>(), PageNumber = 2, PageSize = 50, TotalApprenticeships = 49 };

            var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships2,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage2);

            // Act
            var response = await _sut.Get(query);
            
            // Assert
            response.Apprenticeships.Count().Should().Be(apprenticeships.Count);
            response.MorePages.Should().BeFalse();
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(2);
        }
    }
}
