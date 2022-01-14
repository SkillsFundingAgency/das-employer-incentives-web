using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        private Mock<IOptions<ExternalLinksConfiguration>> _mockConfiguration;
        private string _manageApprenticeshipSiteUrl;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _applicationService = new Mock<IApplicationService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _hashingService = new Mock<IHashingService>();
            _validator = new Mock<IEmploymentStartDateValidator>();
            _mockConfiguration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _manageApprenticeshipSiteUrl = $"http://{Guid.NewGuid()}";
            _mockConfiguration.Setup(m => m.Value).Returns(new ExternalLinksConfiguration { ManageApprenticeshipSiteUrl = _manageApprenticeshipSiteUrl });

            _sut = new ApplyEmploymentDetailsController(_applicationService.Object, _legalEntitiesService.Object,
                _hashingService.Object, _validator.Object, _mockConfiguration.Object);
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

            for(var index = 0; index < model.Apprentices.Count; index++)
            {
                model.Apprentices.Single(x => x.ApprenticeshipId == request.ApprenticeshipIds[index]).EmploymentStartDateDay.Should().Be(request.EmploymentStartDateDays[index].Value);
                model.Apprentices.Single(x => x.ApprenticeshipId == request.ApprenticeshipIds[index]).EmploymentStartDateMonth.Should().Be(request.EmploymentStartDateMonths[index].Value);
                model.Apprentices.Single(x => x.ApprenticeshipId == request.ApprenticeshipIds[index]).EmploymentStartDateYear.Should().Be(request.EmploymentStartDateYears[index].Value);
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
                apprentices, _fixture.Create<bool>(), false);

            _applicationService.Setup(x => x.Get(request.AccountId, request.ApplicationId, true)).ReturnsAsync(application);
            _applicationService.Setup(x => x.Get(request.AccountId, request.ApplicationId, false)).ReturnsAsync(application);
          
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(request.AccountId, request.AccountLegalEntityId))
                .ReturnsAsync(legalEntity);

            _validator.Setup(x => x.Validate(request)).Returns(Enumerable.Empty<DateValidationResult>());

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

        [Test]
        public async Task Then_the_sign_new_agreement_page_is_displayed_when_a_new_agreement_needs_to_be_signed()
        {
            // Arrange
            var apprentices = _fixture.CreateMany<ApplicationApprenticeshipModel>().ToList();

            var application = new ApplicationModel(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(),
                apprentices, _fixture.Create<bool>(), true);

            var request = _fixture.Build<EmploymentStartDatesRequest>()
                    .With(x => x.EmploymentStartDateDays, new List<int?>())
                    .With(x => x.EmploymentStartDateMonths, new List<int?>())
                    .With(x => x.EmploymentStartDateYears, new List<int?>())
                    .Create();

            _applicationService.Setup(x => x.Get(request.AccountId, request.ApplicationId, true)).ReturnsAsync(application);
            _applicationService.Setup(x => x.Get(request.AccountId, request.ApplicationId, false)).ReturnsAsync(application);

            var legalEntity = _fixture.Create<LegalEntityModel>();
            _legalEntitiesService.Setup(x => x.Get(request.AccountId, application.AccountLegalEntityId))
                .ReturnsAsync(legalEntity);

            _validator.Setup(x => x.Validate(request)).Returns(Enumerable.Empty<DateValidationResult>());

            // Act
            var result = await _sut.SubmitEmploymentStartDates(request) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as NewAgreementRequiredViewModel;
            model.AccountsAgreementsUrl.Should().Be($"{ _manageApprenticeshipSiteUrl}/accounts/{request.AccountId}/agreements");
            model.AccountId.Should().Be(request.AccountId);
            model.ApplicationId.Should().Be(request.ApplicationId);
            model.OrganisationName.Should().Be(legalEntity.Name);
            model.Title.Should().Be($"{legalEntity.Name} needs to accept a new employer agreement");
        }
    }
}
