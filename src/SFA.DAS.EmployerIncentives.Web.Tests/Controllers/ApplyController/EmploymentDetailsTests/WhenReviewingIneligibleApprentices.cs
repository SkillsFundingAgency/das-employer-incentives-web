using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.EmploymentDetailsTests
{
    [TestFixture]
    public class WhenReviewingIneligibleApprentices
    {
        private Guid _applicationId;
        private string _hashedAccountId;
        private Mock<IApprenticesService> _apprenticesServiceMock;
        private Mock<IApplicationService> _applicationServiceMock;
        private Web.Controllers.ApplyApprenticeshipsController _sut;
        private Fixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _applicationId = Guid.NewGuid();
            _hashedAccountId = Guid.NewGuid().ToString();
            _apprenticesServiceMock = new Mock<IApprenticesService>();
            _applicationServiceMock = new Mock<IApplicationService>();

            _sut = new Web.Controllers.ApplyApprenticeshipsController(_apprenticesServiceMock.Object,
                _applicationServiceMock.Object, Mock.Of<ILegalEntitiesService>());
        }

        [Test]
        public async Task Then_the_employment_start_dates_are_submitted()
        {
            // Arrange
            var apprentices = _fixture.Build<ApplicationApprenticeshipModel>()
                .With(a => a.HasEligibleEmploymentStartDate, true).CreateMany(3).ToList();
            apprentices[0].HasEligibleEmploymentStartDate = false;

            var application = new ApplicationModel(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(),
                apprentices, _fixture.Create<bool>(), _fixture.Create<bool>());
            _applicationServiceMock.Setup(x => x.Get(_hashedAccountId, _applicationId, true, false)).ReturnsAsync(application);


            // Act
            var result = await _sut.ConfirmApprenticeships(_hashedAccountId, _applicationId, false) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as ApplicationConfirmationViewModel;
            Assert.IsNotNull(model);
            model.Apprentices.Count.Should().Be(2);
            model.Apprentices.Any(a => !a.HasEligibleEmploymentStartDate).Should().BeFalse();
            _applicationServiceMock.Verify(x => x.Get(_hashedAccountId, _applicationId, true, false), Times.Once());
        }
    }
}
