using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.BankDetails;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.BankDetailsStatusServiceTests
{
    [TestFixture]
    public class WhenApplicationCompleted
    {
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private BankDetailsStatusService _sut;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;

        [SetUp]
        public void Arrange()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _sut = new BankDetailsStatusService(_legalEntitiesService.Object);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Then_the_bank_details_status_is_updated_if_not_populated(string existingStatus)
        {
            // Arrange
            var legalEntity = _fixture.Create<LegalEntityModel>();
            legalEntity.AccountId = _accountId;
            legalEntity.AccountLegalEntityId = _accountLegalEntityId;
            legalEntity.VrfCaseStatus = existingStatus;
            _legalEntitiesService.Setup(x => x.Get(_accountId, _accountLegalEntityId)).ReturnsAsync(legalEntity);

            // Act
            var updatedLegalEntity = await _sut.RecordBankDetailsComplete(_accountId, _accountLegalEntityId);

            // Assert
            updatedLegalEntity.VrfCaseStatus.Should().Be("Requested");
            _legalEntitiesService.Verify(x => x.UpdateVrfCaseStatus(It.Is<LegalEntityModel>(x => x.AccountId == _accountId && x.AccountLegalEntityId == _accountLegalEntityId && x.VrfCaseStatus == "Requested")), Times.Once);
        }

        [Test]
        public async Task Then_the_bank_details_status_is_not_updated_if_already_populated()
        {
            // Arrange
            var legalEntity = _fixture.Create<LegalEntityModel>();
            legalEntity.AccountId = _accountId;
            legalEntity.AccountLegalEntityId = _accountLegalEntityId;
            legalEntity.VrfCaseStatus = "To Process";
            _legalEntitiesService.Setup(x => x.Get(_accountId, _accountLegalEntityId)).ReturnsAsync(legalEntity);

            // Act
            var storedLegalEntity = await _sut.RecordBankDetailsComplete(_accountId, _accountLegalEntityId);

            // Assert
            storedLegalEntity.VrfCaseStatus.Should().Be(legalEntity.VrfCaseStatus);
            _legalEntitiesService.Verify(x => x.UpdateVrfCaseStatus(It.Is<LegalEntityModel>(x => x.AccountId == _accountId && x.AccountLegalEntityId == _accountLegalEntityId)), Times.Never);
        }
    }
}
