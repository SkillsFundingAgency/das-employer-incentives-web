using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Components.Forms;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Session;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.PaginationTests
{
    [TestFixture]
    public class WhenSavingPagingInformation
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
        }

        [Test]
        public async Task Then_the_paging_information_is_saved_to_session()
        {
            // Arrange
            var pagingInformation = _fixture.Create<PagingInformation>();

            // Act
            await _sut.SavePagingInformation(pagingInformation);

            // Assert
            var expectedSessionKey = string.Format(SessionKeyFormatString, pagingInformation.StartIndex);
            _sessionService.Verify(x => x.Set(expectedSessionKey, pagingInformation), Times.Once);
        }
    }
}
