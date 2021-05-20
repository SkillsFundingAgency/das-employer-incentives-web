
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Session;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.PaginationTests
{
    [TestFixture]
    public class WhenGettingPagingInformation
    {
        private Mock<ISessionService> _sessionService;
        private Mock<IPaginationQuery> _query;
        private PaginationService _sut;
        private string SessionKeyFormatString = "ApprenticeshipsPage_{0}";
        private Fixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _sessionService = new Mock<ISessionService>();
            _query = new Mock<IPaginationQuery>();
            _sut = new PaginationService(_sessionService.Object);

            _query.Setup(x => x.PageSize).Returns(50);
        }

        [Test]
        public async Task Then_the_pagination_information_is_created_if_not_already_stored_in_session()
        {
            // Arrange
            var startIndex = 1;
            var offset = 0;

            _query.Setup(x => x.StartIndex).Returns(startIndex);
            _query.Setup(x => x.Offset).Returns(offset);

            // Act
            var paginationInfo = await _sut.GetPagingInformation(_query.Object);

            // Assert
            paginationInfo.Should().NotBeNull();
            paginationInfo.PageNumber.Should().Be(1);
            paginationInfo.StartIndex.Should().Be(startIndex);
            paginationInfo.Offset.Should().Be(offset);
        }
    
        [Test]
        public async Task Then_the_page_number_is_starts_at_the_previous_page_if_the_offset_is_greater_than_zero()
        {
            // Arrange
            var startIndex = 51;
            var offset = 2;

            var pagingInformation = new PagingInformation {PageNumber = 1, Offset = 0, StartIndex = 1};
            _sessionService.Setup(x => x.Get<PagingInformation>(string.Format(SessionKeyFormatString, "1")))
                .Returns(pagingInformation);
            _sessionService.Setup(x => x.Set(string.Format(SessionKeyFormatString, "1"),
                It.Is<PagingInformation>(y => y.StartIndex == startIndex)));

            _query.Setup(x => x.StartIndex).Returns(startIndex);
            _query.Setup(x => x.Offset).Returns(offset);

            // Act
            var paginationInfo = await _sut.GetPagingInformation(_query.Object);

            // Assert
            paginationInfo.Should().NotBeNull();
            paginationInfo.PageNumber.Should().Be(paginationInfo.PageNumber);
            paginationInfo.StartIndex.Should().Be(startIndex);
            paginationInfo.Offset.Should().Be(offset);
            _sessionService.Verify(x => x.Set(string.Format(SessionKeyFormatString, "51"),
                It.Is<PagingInformation>(y => y.StartIndex == startIndex)), Times.Once);
        }


        [Test]
        public async Task Then_the_page_number_is_starts_at_the_next_page_if_the_offset_is_zero()
        {
            // Arrange
            var startIndex = 51;
            var offset = 0;

            var previousPageInformation = new PagingInformation { PageNumber = 1, Offset = 0, StartIndex = 1 };
            _sessionService.Setup(x => x.Get<PagingInformation>(string.Format(SessionKeyFormatString, "1")))
                .Returns(previousPageInformation);

            _query.Setup(x => x.StartIndex).Returns(startIndex);
            _query.Setup(x => x.Offset).Returns(offset);

            // Act
            var paginationInfo = await _sut.GetPagingInformation(_query.Object);

            // Assert
            paginationInfo.Should().NotBeNull();
            paginationInfo.PageNumber.Should().Be(previousPageInformation.PageNumber + 1);
            paginationInfo.StartIndex.Should().Be(startIndex);
            paginationInfo.Offset.Should().Be(offset);
        }

        [Test]
        public async Task Then_the_paging_information_is_retrieved_from_session_if_previously_calculated()
        {
            // Arrange
            var pagingInformation = new PagingInformation
            {
                StartIndex = _fixture.Create<int>(),
                Offset = _fixture.Create<int>(),
                PageNumber = _fixture.Create<int>()
            };

            var expectedSessionKey = string.Format(SessionKeyFormatString, pagingInformation.StartIndex);
            _sessionService.Setup(x => x.Get<PagingInformation>(expectedSessionKey)).Returns(pagingInformation);

            _query.Setup(x => x.StartIndex).Returns(pagingInformation.StartIndex);

            // Act
            var result = await _sut.GetPagingInformation(_query.Object);

            // Assert
            _sessionService.Verify(x => x.Get<PagingInformation>(expectedSessionKey), Times.Once);
            result.StartIndex.Should().Be(pagingInformation.StartIndex);
            result.Offset.Should().Be(pagingInformation.Offset);
            result.PageNumber.Should().Be(pagingInformation.PageNumber);
        }
    }
}
