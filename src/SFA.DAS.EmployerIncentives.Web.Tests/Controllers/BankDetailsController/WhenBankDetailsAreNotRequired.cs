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
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.BankDetailsController
{ 
    [TestFixture]
    public class WhenBankDetailsAreNotRequired
    {
        private Mock<IVerificationService> _verificationService;
        private Mock<IEmailService> _emailService;
        private Mock<IApplicationService> _applicationService;
        private Mock<IHashingService> _hashingService;
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
            _hashingService = new Mock<IHashingService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();

            _sut = new Web.Controllers.BankDetailsController(_verificationService.Object, _emailService.Object,
                _applicationService.Object, _hashingService.Object, _legalEntitiesService.Object);
        }

        [Test]
        public async Task Then_the_user_is_redirected_to_the_application_complete_page()
        {
            // Arrange
            var application = new ApplicationModel(_applicationId, _accountId, _fixture.Create<string>(),
                _fixture.CreateMany<ApplicationApprenticeshipModel>(1), 
                bankDetailsRequired: false,
                newAgreementRequired: false);

            _applicationService.Setup(x => x.Get(_accountId, _applicationId, false)).ReturnsAsync(application);

            // Act
            var redirectResult = await _sut.BankDetailsConfirmation(_accountId, _applicationId) as RedirectToActionResult;

            // Assert
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Confirmation");
            redirectResult.ControllerName.Should().Be("ApplicationComplete");
        }
    }
}
