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
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.PaymentsController
{
    [TestFixture]
    public class WhenPaymentApplicationsAccessed
    {
        private Web.Controllers.PaymentsController _sut;
        private Mock<IApprenticeshipIncentiveService> _apprenticeshipIncentiveService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IHashingService> _hashingService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;
        private string _sortOrder;
        private string _sortField;

        [SetUp]
        public void Arrange()
        {
            _apprenticeshipIncentiveService = new Mock<IApprenticeshipIncentiveService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _hashingService = new Mock<IHashingService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _sut = new Web.Controllers.PaymentsController(_apprenticeshipIncentiveService.Object, _legalEntitiesService.Object, _hashingService.Object, _configuration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
            _sortOrder = ApplicationsSortOrder.Ascending;
            _sortField = ApplicationsSortField.ApprenticeName;
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns(_fixture.Create<string>());
        }

        [Test]
        public async Task Then_the_view_contains_summary_for_submitted_applications()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(5));

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications, FirstSubmittedApplicationId = Guid.NewGuid()};
            _apprenticeshipIncentiveService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.Count().Should().Be(applications.Count());
            viewModel.AddBankDetailsLink.Should().Be($":///{_accountId}/bank-details/{getApplicationsResponse.FirstSubmittedApplicationId}/add-bank-details");
        }

        [Test]
        public async Task Then_a_shutter_page_is_shown_if_no_applcations()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _apprenticeshipIncentiveService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("NoApplications");
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Then_the_default_sort_order_is_ascending_by_apprentice_name(string orderByText)
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(2));
            applications[0].FirstName = "Steve";
            applications[0].LastName = "Jones";
            applications[1].FirstName = "Freda";
            applications[1].LastName = "Johnson";
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _apprenticeshipIncentiveService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, ApplicationsSortOrder.Ascending, ApplicationsSortField.ApprenticeName) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            var modelApplications = viewModel.Applications.ToArray();
            modelApplications[0].ApprenticeName.Should().Be(applications[1].ApprenticeName);
            modelApplications[1].ApprenticeName.Should().Be(applications[0].ApprenticeName);
        }

        [Test]
        public async Task Then_applications_are_sorted_by_application_date_descending()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(2));
            applications[0].ApplicationDate = new DateTime(2020, 09, 01);
            applications[1].ApplicationDate = new DateTime(2020, 08, 20);
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _apprenticeshipIncentiveService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, ApplicationsSortOrder.Descending, ApplicationsSortField.ApplicationDate) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            var modelApplications = viewModel.Applications.ToArray();
            modelApplications[1].ApplicationDate.Should().Be(applications[1].ApplicationDate);
            modelApplications[0].ApplicationDate.Should().Be(applications[0].ApplicationDate);
        }

        [Test]
        public async Task Then_applications_are_sorted_by_application_date_ascending()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(2));
            applications[0].ApplicationDate = new DateTime(2020, 09, 01);
            applications[1].ApplicationDate = new DateTime(2020, 08, 20);
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _apprenticeshipIncentiveService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, ApplicationsSortOrder.Ascending, ApplicationsSortField.ApplicationDate) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            var modelApplications = viewModel.Applications.ToArray();
            modelApplications[0].ApplicationDate.Should().Be(applications[1].ApplicationDate);
            modelApplications[1].ApplicationDate.Should().Be(applications[0].ApplicationDate);
        }
    }
}
