﻿using System;
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
            _sut = new ApprenticesService(_httpClient, _hashingService.Object);

            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_the_apprenticeships_are_retrieved_in_a_single_request()
        {
            // Arrange
            var query = new ApprenticesQuery(_accountId, _accountLegalEntityId, pageNumber: 1, pageSize: 50, offset: 0);

            var apprenticeships = _fixture.CreateMany<ApprenticeDto>(49).ToList();
            var eligibleApprenticeships = new EligibleApprenticesDto
                {Apprenticeships = apprenticeships, PageNumber = 1, PageSize = 50, TotalApprenticeships = 49};

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<EligibleApprenticesDto>(eligibleApprenticeships,
                    new JsonMediaTypeFormatter(), "application/json")
            };
            _httpClientHandlerFake.AddResponse(httpResponseMessage);

            // Act
            var response = await _sut.Get(query);
            
            // Assert
            response.Apprenticeships.Count().Should().Be(apprenticeships.Count);
            response.MorePages.Should().BeFalse();
            _httpClientHandlerFake.RequestMesssages.Count.Should().Be(1);
        }
    }
}
