using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.ApplicationCompleteTests
{
    [TestFixture]
    public class WhenApplicationComplete
    {
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IApplicationService> _applicationService;
        private ApplicationCompleteController _sut;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;
        private Guid _applicationId;
        private const string VrfStatusCompleted = "Case Request completed";

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();

            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _applicationService = new Mock<IApplicationService>();
            var application = new ApplicationModel(_applicationId, _accountId, _accountLegalEntityId,
                _fixture.CreateMany<ApplicationApprenticeshipModel>(2), false, false);
            _applicationService.Setup(x => x.Get(_accountId, _applicationId, false)).ReturnsAsync(application);
            
            _sut = new ApplicationCompleteController(_legalEntitiesService.Object, _applicationService.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Then_the_view_indicates_that_the_bank_details_have_been_received(string existingStatus)
        {
            // Arrange
            var legalEntity = _fixture.Create<LegalEntityModel>();
            legalEntity.AccountId = _accountId;
            legalEntity.AccountLegalEntityId = _accountLegalEntityId;
            legalEntity.VrfCaseStatus = existingStatus;
            _legalEntitiesService.Setup(x => x.Get(_accountId, _accountLegalEntityId)).ReturnsAsync(legalEntity);
            
            // Act
            var viewResult = await _sut.Confirmation(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmationViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
            model.OrganisationName.Should().Be(legalEntity.Name);
            model.ShowBankDetailsInReview.Should().BeTrue();
            _legalEntitiesService.Verify(x => x.UpdateVrfCaseStatus(It.Is<LegalEntityModel>(y => y.AccountId == _accountId && y.AccountLegalEntityId == _accountLegalEntityId && y.VrfCaseStatus == "Requested")), Times.Once);
        }

        [Test]
        public async Task Then_the_view_indicates_that_the_bank_details_have_been_approved()
        {
            // Arrange
            var legalEntity = _fixture.Create<LegalEntityModel>();
            legalEntity.AccountId = _accountId;
            legalEntity.AccountLegalEntityId = _accountLegalEntityId;
            legalEntity.VrfCaseStatus = VrfStatusCompleted;
            _legalEntitiesService.Setup(x => x.Get(_accountId, _accountLegalEntityId)).ReturnsAsync(legalEntity);

            // Act
            var viewResult = await _sut.Confirmation(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmationViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
            model.OrganisationName.Should().Be(legalEntity.Name);
            model.ShowBankDetailsInReview.Should().BeFalse();
            _legalEntitiesService.Verify(x => x.UpdateVrfCaseStatus(It.Is<LegalEntityModel>(y => y.AccountId == _accountId && y.AccountLegalEntityId == _accountLegalEntityId)), Times.Never);
        }
    }
}
