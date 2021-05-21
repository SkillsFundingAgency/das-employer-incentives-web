using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyApprenticeshipsController.ConfirmApprenticeshipsTests
{
    [TestFixture]
    public class WhenConfirmApprenticeships
    {
        private Mock<ILegalEntitiesService> _mockLegalEntitiesService;
        private Mock<IApplicationService> _mockApplicationService;
        private Mock<IApprenticesService> _mockApprenticesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _mockConfiguration;
        
        private Web.Controllers.ApplyApprenticeshipsController _sut;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;
        private Guid _applicationId;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();

            _mockApprenticesService = new Mock<IApprenticesService>();
            _mockLegalEntitiesService = new Mock<ILegalEntitiesService>();
            _mockApplicationService = new Mock<IApplicationService>();
            _mockConfiguration = new Mock<IOptions<ExternalLinksConfiguration>>();

            _sut = new Web.Controllers.ApplyApprenticeshipsController(
                _mockApprenticesService.Object,
                _mockApplicationService.Object,
                _mockLegalEntitiesService.Object);
        }

        [Test()]
        public async Task Then_the_apprenticeships_being_applied_for_are_displayed_when_they_have_eligible_employer_start_dates()
        {
            // Arrange
            var legalEntityName = _fixture.Create<string>();
            var apprenticeship1 = _fixture
                .Build<ApplicationApprenticeshipModel>()
                .With(a => a.HasEligibleEmploymentStartDate, true)
                .Create();
            var apprenticeship2 = _fixture
                .Build<ApplicationApprenticeshipModel>()
                .With(a => a.HasEligibleEmploymentStartDate, true)
                .Create();

            var apprentices = new List<ApplicationApprenticeshipModel>
            {
               apprenticeship1,
               apprenticeship2
            };  

            var applicationResponse = new ApplicationModel(
                _applicationId,
                _accountId,
                _accountLegalEntityId,
                apprentices,
                false,
                false);     
            
            _mockApplicationService
                .Setup(m => m.Get(_accountId, _applicationId, true))
                .ReturnsAsync(applicationResponse);
            _mockLegalEntitiesService
                .Setup(m => m.Get(_accountId, _accountLegalEntityId))
                .ReturnsAsync(new LegalEntityModel() { Name = legalEntityName } );

            // Act
            var viewResult = await _sut.ConfirmApprenticeships(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationConfirmationViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
            model.OrganisationName.Should().Be(legalEntityName);
            model.Apprentices.Count.Should().Be(2);
            AssertAreEquivalent(model.Apprentices.Single(a => a.ApprenticeshipId == apprenticeship1.ApprenticeshipId), apprenticeship1);
            AssertAreEquivalent(model.Apprentices.Single(a => a.ApprenticeshipId == apprenticeship2.ApprenticeshipId), apprenticeship2);
        }

        [Test()]
        public async Task Then_the_non_eligible_apprenticeships_being_applied_for_are_displayed_when_all_employer_start_dates_are_non_eligible()
        {
            // Arrange
            var legalEntityName = _fixture.Create<string>();
            var apprenticeship1 = _fixture
                .Build<ApplicationApprenticeshipModel>()
                .With(a => a.HasEligibleEmploymentStartDate, false)
                .Create();
            var apprenticeship2 = _fixture
                .Build<ApplicationApprenticeshipModel>()
                .With(a => a.HasEligibleEmploymentStartDate, false)
                .Create();

            var apprentices = new List<ApplicationApprenticeshipModel>
            {
               apprenticeship1,
               apprenticeship2
            };

            var applicationResponse = new ApplicationModel(
                _applicationId,
                _accountId,
                _accountLegalEntityId,
                apprentices,
                false,
                false);

            _mockApplicationService
                .Setup(m => m.Get(_accountId, _applicationId, true))
                .ReturnsAsync(applicationResponse);
            _mockLegalEntitiesService
                .Setup(m => m.Get(_accountId, _accountLegalEntityId))
                .ReturnsAsync(new LegalEntityModel() { Name = legalEntityName });

            // Act
            var viewResult = await _sut.ConfirmApprenticeships(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as NotEligibleViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
            model.OrganisationName.Should().Be(legalEntityName);
            model.Apprentices.Count.Should().Be(2);
            model.AllInEligible.Should().BeTrue();
            AssertAreEquivalent(model.Apprentices.Single(a => a.ApprenticeshipId == apprenticeship1.ApprenticeshipId), apprenticeship1);
            AssertAreEquivalent(model.Apprentices.Single(a => a.ApprenticeshipId == apprenticeship2.ApprenticeshipId), apprenticeship2);
        }

        [Test()]
        public async Task Then_the_non_eligible_apprenticeships_being_applied_for_are_displayed_when_some_employer_start_dates_are_non_eligible()
        {
            // Arrange
            var legalEntityName = _fixture.Create<string>();
            var apprenticeship1 = _fixture
                .Build<ApplicationApprenticeshipModel>()
                .With(a => a.HasEligibleEmploymentStartDate, true)
                .Create();
            var apprenticeship2 = _fixture
                .Build<ApplicationApprenticeshipModel>()
                .With(a => a.HasEligibleEmploymentStartDate, false)
                .Create();

            var apprentices = new List<ApplicationApprenticeshipModel>
            {
               apprenticeship1,
               apprenticeship2
            };

            var applicationResponse = new ApplicationModel(
                _applicationId,
                _accountId,
                _accountLegalEntityId,
                apprentices,
                false,
                false);

            _mockApplicationService
                .Setup(m => m.Get(_accountId, _applicationId, true))
                .ReturnsAsync(applicationResponse);
            _mockLegalEntitiesService
                .Setup(m => m.Get(_accountId, _accountLegalEntityId))
                .ReturnsAsync(new LegalEntityModel() { Name = legalEntityName });

            // Act
            var viewResult = await _sut.ConfirmApprenticeships(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as NotEligibleViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
            model.OrganisationName.Should().Be(legalEntityName);
            model.Apprentices.Count.Should().Be(1);
            model.AllInEligible.Should().BeFalse();
            AssertAreEquivalent(model.Apprentices.Single(a => a.ApprenticeshipId == apprenticeship2.ApprenticeshipId), apprenticeship2);            
        }

        private void AssertAreEquivalent(ApplicationApprenticeship applicationApprenticeship, ApplicationApprenticeshipModel model)
        {
            applicationApprenticeship.ApprenticeshipId.Should().Be(model.ApprenticeshipId);
            applicationApprenticeship.CourseName.Should().Be(model.CourseName);
            applicationApprenticeship.DisplayName.Should().Be(model.FullName);
            applicationApprenticeship.EmploymentStartDate.Should().Be(model.EmploymentStartDate);
            applicationApprenticeship.ExpectedAmount.Should().Be(model.ExpectedAmount);
            applicationApprenticeship.HasEligibleEmploymentStartDate.Should().Be(model.HasEligibleEmploymentStartDate);
            applicationApprenticeship.StartDate.Should().Be(model.StartDate);
            applicationApprenticeship.FirstName.Should().Be(model.FirstName);
            applicationApprenticeship.LastName.Should().Be(model.LastName);
            applicationApprenticeship.Uln.Should().Be(model.Uln);
        }
    }
}
