using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.BankDetailsController
{
    [TestFixture]
    public class WhenAddBankDetailsIsAccessed
    {
        private Mock<IVerificationService> _verificationService;
        private Mock<IEmailService> _emailService;
        private Mock<IApplicationService> _applicationService;
        private Mock<IHashingService> _hashingService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
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
            _hashingService = new Mock<IHashingService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _sut = new Web.Controllers.BankDetailsController(_verificationService.Object, _emailService.Object, _applicationService.Object,
                                                             _hashingService.Object, _legalEntitiesService.Object, _configuration.Object);
            var urlHelper = new Mock<IUrlHelper>();
            _sut.Url = urlHelper.Object;
            _sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        }

        [Test]
        public async Task Then_the_redirect_to_the_achieve_service_is_generated_for_the_new_vendor_journey()
        {
            // Arrange
            var application = _fixture.Create<ApplicationConfirmationViewModel>();
            _applicationService.Setup(x => x.Get(_accountId, _applicationId, false)).ReturnsAsync(application);

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
