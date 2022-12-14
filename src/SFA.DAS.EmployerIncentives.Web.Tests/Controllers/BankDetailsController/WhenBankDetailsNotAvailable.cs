using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.Email.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.BankDetailsController
{
    [TestFixture]
    public class WhenBankDetailsNotAvailable
    {
        private Mock<IVerificationService> _verificationService;
        private Mock<IEmailService> _emailService;
        private Mock<IApplicationService> _applicationService;
        private Mock<IAccountEncodingService> _encodingService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Fixture _fixture;
        private Web.Controllers.BankDetailsController _sut;
        private string _accountId;
        private long _accountLegalEntityId;
        private Guid _applicationId;
        private string _emailAddress;

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
            _accountLegalEntityId = _fixture.Create<long>();
            _applicationId = Guid.NewGuid();
            _emailAddress = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_a_reminder_email_is_sent_to_the_user_so_they_can_rejoin_the_bank_details_journey()
        {
            // Arrange
            var model = _fixture.Create<BankDetailsConfirmationViewModel>();
            model.CanProvideBankDetails = false;
            _applicationService.Setup(x => x.GetApplicationLegalEntity(_accountId, _applicationId)).ReturnsAsync(_accountLegalEntityId);
            _emailService.Setup(x => x.SendBankDetailsRequiredEmail(It.Is<SendBankDetailsEmailRequest>(x => x.EmailAddress == _emailAddress))).Returns(Task.CompletedTask);

            var claims = new Claim[]
            {
                new Claim(EmployerClaimTypes.EmailAddress, _emailAddress)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _sut = new Web.Controllers.BankDetailsController(_verificationService.Object, _emailService.Object,
                _applicationService.Object, _encodingService.Object, _legalEntitiesService.Object)
            {
                ControllerContext = new ControllerContext()
            };
            _sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // Act
            var result = await _sut.BankDetailsConfirmation(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("NeedBankDetails");

            _emailService.Verify(x => x.SendBankDetailsRequiredEmail(It.Is<SendBankDetailsEmailRequest>(x => x.EmailAddress == _emailAddress)), Times.Once);
        }
    }
}
