using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.ConfirmationTests
{
    [TestFixture]
    public class WhenApprenticesForApplicationConfirmed
    {
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private Mock<IApplicationService> _applicationService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Web.Controllers.ApplyController _sut;
        private Fixture _fixture;
        private string _accountId;
        private Guid _applicationId;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _applicationService = new Mock<IApplicationService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();
            _sut = new Web.Controllers.ApplyController(_configuration.Object, _applicationService.Object, _legalEntitiesService.Object);
        }

        [Test]
        public async Task Then_the_user_is_asked_to_confirm_the_declaration()
        {
            // Arrange
            var application = _fixture.Create<ApplicationModel>();
            _applicationService.Setup(x => x.Get(_accountId, _applicationId, false)).ReturnsAsync(application);
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(_accountId, application.AccountLegalEntityId)).ReturnsAsync(legalEntity);
            
            // Act
            var viewResult = await _sut.Declaration(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as DeclarationViewModel;
            model.AccountId.Should().Be(_accountId);
            model.ApplicationId.Should().Be(_applicationId);
            model.OrganisationName.Should().Be(legalEntity.Name);
        }
    }
}
