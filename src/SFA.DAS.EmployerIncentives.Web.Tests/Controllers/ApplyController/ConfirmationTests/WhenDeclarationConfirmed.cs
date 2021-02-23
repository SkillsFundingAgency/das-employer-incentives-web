using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.ConfirmationTests
{
    [TestFixture]
    public class WhenDeclarationConfirmed
    {
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private Mock<IApplicationService> _applicationService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Web.Controllers.ApplyController _sut;
        private Fixture _fixture;
        private string _accountId;
        private Guid _applicationId;
        private string _emailAddress;
        private string _givenName;
        private string _familyName;
        private string _userName;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _applicationService = new Mock<IApplicationService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();
            _emailAddress = _fixture.Create<string>();
            _givenName = _fixture.Create<string>();
            _familyName = _fixture.Create<string>();
            _userName = $"{_givenName} {_familyName}";
        }

        [Test]
        public async Task Then_the_application_is_submitted()
        {
            // Arrange
            var claims = new Claim[]
            {
                new Claim(EmployerClaimTypes.EmailAddress, _emailAddress),
                new Claim(EmployerClaimTypes.GivenName, _givenName),
                new Claim(EmployerClaimTypes.FamilyName, _familyName)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _applicationService.Setup(x => x.Confirm(_accountId, _applicationId, _emailAddress, _userName)).Returns(Task.CompletedTask);

            _sut = new Web.Controllers.ApplyController(_configuration.Object, _applicationService.Object, _legalEntitiesService.Object)
            {
                ControllerContext = new ControllerContext()
            };
            _sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // Act 
            var redirectResult = await _sut.SubmitApplication(_accountId, _applicationId) as RedirectToActionResult;

            // Assert
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("BankDetailsConfirmation");
            redirectResult.ControllerName.Should().Be("BankDetails");

            _applicationService.Verify(x => x.Confirm(_accountId, _applicationId, _emailAddress, _userName), Times.Once);
        }
    }
}
