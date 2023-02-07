using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.BankDetailsController
{
    [TestFixture]
    public class WhenBankDetailsAreRequired
    {
        private Mock<IVerificationService> _verificationService;
        private Mock<IEmailService> _emailService;
        private Mock<IApplicationService> _applicationService;
        private Mock<IAccountEncodingService> _encodingService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Fixture _fixture;
        private Web.Controllers.BankDetailsController _sut;
        private string _accountId;
        private Guid _applicationId;

        [SetUp]
        public void Setup()
        {
            _verificationService = new Mock<IVerificationService>();
            _emailService = new Mock<IEmailService>();
            _applicationService = new Mock<IApplicationService>();
            _encodingService = new Mock<IAccountEncodingService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();

            _sut = new Web.Controllers.BankDetailsController(_verificationService.Object, _emailService.Object,
                _applicationService.Object, _encodingService.Object, _legalEntitiesService.Object);
        }

        [Test]
        public async Task Then_the_user_is_asked_to_confirm_they_can_provide_bank_details()
        {
            // Arrange
            var application = new ApplicationModel(_applicationId, _accountId, _fixture.Create<string>(),
                _fixture.CreateMany<ApplicationApprenticeshipModel>(1),
                bankDetailsRequired: true,
                newAgreementRequired: false);

            _applicationService.Setup(x => x.Get(_accountId, _applicationId, false, true)).ReturnsAsync(application);
            
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(_accountId, application.AccountLegalEntityId)).ReturnsAsync(legalEntity);

            // Act
            var viewResult = await _sut.BankDetailsConfirmation(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as BankDetailsConfirmationViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.ApplicationId.Should().Be(_applicationId);
            model.CanProvideBankDetails.Should().BeNull();
            model.OrganisationName.Should().Be(legalEntity.Name);
        }
    }
}
