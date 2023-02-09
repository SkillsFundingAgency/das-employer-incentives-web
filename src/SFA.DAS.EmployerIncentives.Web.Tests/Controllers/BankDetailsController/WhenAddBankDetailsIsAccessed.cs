using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.BankDetailsController
{
    [TestFixture]
    public class WhenAddBankDetailsIsAccessed
    {
        private Mock<IVerificationService> _verificationService;
        private Mock<IEmailService> _emailService;
        private Mock<IApplicationService> _applicationService;
        private Mock<IAccountEncodingService> _encodingService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Web.Controllers.BankDetailsController _sut;
        private Fixture _fixture;
        private string _accountId;
        private Guid _applicationId;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();

            _verificationService = new Mock<IVerificationService>();
            _emailService = new Mock<IEmailService>();
            _applicationService = new Mock<IApplicationService>();
            _encodingService = new Mock<IAccountEncodingService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _sut = new Web.Controllers.BankDetailsController(_verificationService.Object, _emailService.Object, _applicationService.Object,
                                                             _encodingService.Object, _legalEntitiesService.Object);
            var urlHelper = new Mock<IUrlHelper>();
            _sut.Url = urlHelper.Object;
            _sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        }

        [Test]
        public async Task Then_the_redirect_to_the_achieve_service_is_generated_for_the_new_vendor_journey()
        {
            // Arrange
            var application = _fixture.Create<ApplicationModel>();
            _applicationService.Setup(x => x.Get(_accountId, _applicationId, false, true)).ReturnsAsync(application);

            var achieveServiceUrl = _fixture.Create<string>();
            _verificationService.Setup(x => x.BuildAchieveServiceUrl(_accountId, application.AccountLegalEntityId, _applicationId, It.IsAny<string>(), false)).ReturnsAsync(achieveServiceUrl);

            // Act
            var redirectResult = await _sut.EnterBankDetails(_accountId, _applicationId) as RedirectResult;

            // Assert
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(achieveServiceUrl);
        }
    }
}
