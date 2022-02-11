using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Validators;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.EmploymentDetailsTests
{
    [TestFixture]
    public class WhenRequestingEmploymentStartDatesForApprentices
    {
        private Mock<IApplicationService> _applicationService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IHashingService> _hashingService;
        private Mock<IEmploymentStartDateValidator> _validator;
        private ApplyEmploymentDetailsController _sut;
        private Fixture _fixture;
        private string _accountId;
        private Guid _applicationId;
        private Mock<IOptions<ExternalLinksConfiguration>> _mockConfiguration;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _applicationService = new Mock<IApplicationService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _hashingService = new Mock<IHashingService>();
            _validator = new Mock<IEmploymentStartDateValidator>();
            _mockConfiguration = new Mock<IOptions<ExternalLinksConfiguration>>();

            _sut = new ApplyEmploymentDetailsController(_applicationService.Object, _legalEntitiesService.Object,
                _hashingService.Object, _validator.Object, _mockConfiguration.Object);

            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();
        }

        [Test]
        public async Task Then_the_apprentices_previously_selected_are_displayed()
        {
            // Arrange
            var application = _fixture.Create<ApplicationModel>();
            _applicationService.Setup(x => x.Get(_accountId, _applicationId, true, false)).ReturnsAsync(application);
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(_accountId, application.AccountLegalEntityId))
                .ReturnsAsync(legalEntity);
            
            // Act
            var viewResult = await _sut.EmploymentStartDates(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            model.Apprentices.Count.Should().Be(application.Apprentices.Count);
        }

        [Test]
        public async Task Then_any_previously_entered_start_dates_are_displayed()
        {
            // Arrange
            var application = _fixture.Create<ApplicationModel>();
            foreach(var apprentice in application.Apprentices)
            {
                apprentice.EmploymentStartDate = _fixture.Create<DateTime>();
            }
            _applicationService.Setup(x => x.Get(_accountId, _applicationId, true, false)).ReturnsAsync(application);
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(_accountId, application.AccountLegalEntityId))
                .ReturnsAsync(legalEntity);

            // Act
            var viewResult = await _sut.EmploymentStartDates(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            model.Apprentices.Count.Should().Be(application.Apprentices.Count);
            foreach(var apprentice in model.Apprentices)
            {
                apprentice.EmploymentStartDateDay.Should().Be(apprentice.EmploymentStartDate.Value.Day);
                apprentice.EmploymentStartDateMonth.Should().Be(apprentice.EmploymentStartDate.Value.Month);
                apprentice.EmploymentStartDateYear.Should().Be(apprentice.EmploymentStartDate.Value.Year);
            }
        }

        [Test]
        public async Task Then_the_caller_is_redirected_to_the_home_page_when_the_application_has_already_been_submitted()
        {
            _applicationService
                .Setup(x => x.Get(_accountId, _applicationId, false, false))
                .ReturnsAsync(null as ApplicationModel);

            var result = _sut.EmploymentStartDates(_accountId, _applicationId);
            var redirectResult = await result as RedirectToActionResult;

            redirectResult.Should().NotBeNull();
            redirectResult?.ActionName.Should().Be("Home");
            redirectResult?.ControllerName.Should().Be("Home");
        }
    }
}
