using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Validators;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.EmploymentDetailsTests
{
    [TestFixture]
    public class WhenSubmittingEmploymentStartDatesForApprentices
    {
        private Mock<IApplicationService> _applicationService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IHashingService> _hashingService;
        private Mock<IEmploymentStartDateValidator> _validator;
        private ApplyEmploymentDetailsController _sut;
        private Fixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _applicationService = new Mock<IApplicationService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _hashingService = new Mock<IHashingService>();
            _validator = new Mock<IEmploymentStartDateValidator>();

            _sut = new ApplyEmploymentDetailsController(_applicationService.Object, _legalEntitiesService.Object,
                _hashingService.Object, _validator.Object);
        }

        [Test]
        public async Task Then_an_error_is_displayed_if_any_dates_are_invalid()
        {
            // Arrange
            var apprentices = _fixture.CreateMany<ApplicationApprenticeshipModel>(3).ToList();

            var request = _fixture.Create<EmploymentStartDatesRequest>();
            request.ApprenticeshipIds = new List<string>
            {
                apprentices[0].ApprenticeshipId,
                apprentices[1].ApprenticeshipId,
                apprentices[2].ApprenticeshipId
            };
            request.EmploymentStartDateDays = new List<int?>
            {
                2,
                32,
                16
            };
            request.EmploymentStartDateMonths = new List<int?>
            {
                1,
                11,
                13
            };
            request.EmploymentStartDateYears = new List<int?>
            {
                2021,
                2021,
                2021
            };

            var application = new ApplicationModel(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(),
                apprentices, _fixture.Create<bool>(), _fixture.Create<bool>());
            _applicationService.Setup(x => x.Get(request.AccountId, request.ApplicationId, true)).ReturnsAsync(application);
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(request.AccountId, request.AccountLegalEntityId))
                .ReturnsAsync(legalEntity);
            var validationResults = new List<DateValidationResult>
            {
                new DateValidationResult
                    {Index = 0, ValidationMessage = EmploymentStartDateValidator.InvalidFieldErrorMessage}
            };
            _validator.Setup(x => x.Validate(request)).Returns(validationResults);

            // Act
            var viewResult = await _sut.SubmitEmploymentStartDates(request) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            model.DateValidationResults.Count.Should().Be(1);
            //foreach (var apprentice in model.Apprentices)
            for(var index = 0; index < model.Apprentices.Count; index++)
            {
                model.Apprentices[index].EmploymentStartDateDay.Should().Be(request.EmploymentStartDateDays[index].Value);
                model.Apprentices[index].EmploymentStartDateMonth.Should().Be(request.EmploymentStartDateMonths[index].Value);
                model.Apprentices[index].EmploymentStartDateYear.Should().Be(request.EmploymentStartDateYears[index].Value);
            }
        }

        [Test]
        public async Task Then_the_employment_start_dates_are_submitted()
        {
            // Arrange
            var apprentices = _fixture.CreateMany<ApplicationApprenticeshipModel>(3).ToList();

            var request = _fixture.Create<EmploymentStartDatesRequest>();
            request.ApprenticeshipIds = new List<string>
            {
                apprentices[0].ApprenticeshipId,
                apprentices[1].ApprenticeshipId,
                apprentices[2].ApprenticeshipId
            };
            request.EmploymentStartDateDays = new List<int?>
            {
                2,
                28
            };
            request.EmploymentStartDateMonths = new List<int?>
            {
                1,
                3
            };
            request.EmploymentStartDateYears = new List<int?>
            {
                2021,
                2021
            };

            var application = new ApplicationModel(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(),
                apprentices, _fixture.Create<bool>(), _fixture.Create<bool>());
            _applicationService.Setup(x => x.Get(request.AccountId, request.ApplicationId, true)).ReturnsAsync(application);
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(request.AccountId, request.AccountLegalEntityId))
                .ReturnsAsync(legalEntity);
            var validationResults = new List<DateValidationResult>();
            _validator.Setup(x => x.Validate(request)).Returns(validationResults);

            // Act
            var redirectResult = await _sut.SubmitEmploymentStartDates(request) as RedirectToActionResult;

            // Assert
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ConfirmApprenticeships");
            redirectResult.ControllerName.Should().Be("ApplyApprenticeships");
            _applicationService.Verify(x => x.SaveApprenticeshipDetails(
                It.Is<ApprenticeshipDetailsRequest>(y => y.ApplicationId == application.ApplicationId
                && y.ApprenticeshipDetails.Count == request.EmploymentStartDateDays.Count)), Times.Once);
        }
    }
}
