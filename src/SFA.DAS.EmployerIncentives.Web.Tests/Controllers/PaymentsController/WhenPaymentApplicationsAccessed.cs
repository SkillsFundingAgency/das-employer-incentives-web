﻿using AutoFixture;
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
        private Mock<IApplicationService> _applicationService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _linksConfiguration;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;
        private string _sortOrder;
        private string _sortField;
        private string _manageApprenticeshipSiteUrl;
        private string _filter;

        [SetUp]
        public void Arrange()
        {
            _applicationService = new Mock<IApplicationService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _linksConfiguration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _manageApprenticeshipSiteUrl = $"http://{Guid.NewGuid()}";

            _linksConfiguration.Setup(m => m.Value).Returns(new ExternalLinksConfiguration
            {
                ManageApprenticeshipSiteUrl = _manageApprenticeshipSiteUrl
            });

            _sut = new Web.Controllers.PaymentsController(_applicationService.Object, _legalEntitiesService.Object, _linksConfiguration.Object)
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
            _filter = "All";
        }

        [Test]
        public async Task Then_the_view_contains_summary_for_submitted_applications()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(5));

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications, FirstSubmittedApplicationId = Guid.NewGuid()};
            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.Count().Should().Be(applications.Count());
            viewModel.AddBankDetailsLink.Should().Be($":///{_accountId}/bank-details/{getApplicationsResponse.FirstSubmittedApplicationId}/add-bank-details");
        }

        [Test]
        public async Task Then_a_shutter_page_is_shown_if_no_applications()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("NoApplications");
        }

        [TestCase(null, null, false)]
        [TestCase(null, true, true)]
        [TestCase(true, null, true)]
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public async Task Then_accept_new_employer_agreement_is_shown_if_an_application_has_payment_status_that_requires_new_agreement(
            bool? firstPaymentStatusRequiresNewAgreement,
            bool? secondPaymentStatusRequiresNewAgreement,
            bool showAcceptNewEmployerAgreement)
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.Add(
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.FirstPaymentStatus, firstPaymentStatusRequiresNewAgreement.HasValue ? _fixture.Build<PaymentStatusModel>().With(p => p.RequiresNewEmployerAgreement, firstPaymentStatusRequiresNewAgreement).Create() : null)
                .With(p => p.SecondPaymentStatus, secondPaymentStatusRequiresNewAgreement.HasValue ? _fixture.Build<PaymentStatusModel>().With(p => p.RequiresNewEmployerAgreement, secondPaymentStatusRequiresNewAgreement).Create() : null)
                .Create());

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.ShowAcceptNewEmployerAgreement.Should().Be(showAcceptNewEmployerAgreement);
            viewModel.ViewAgreementLink = $"{_manageApprenticeshipSiteUrl}/accounts/{_accountId}/agreements";
            if(firstPaymentStatusRequiresNewAgreement.HasValue)
            {
                viewModel.Applications.First().FirstPaymentStatus.RequiresNewEmployerAgreement = firstPaymentStatusRequiresNewAgreement.Value;
                viewModel.Applications.First().FirstPaymentStatus.ViewAgreementLink = $"{_manageApprenticeshipSiteUrl}/accounts/{_accountId}/agreements";
            }
            else
            {
                viewModel.Applications.First().FirstPaymentStatus.Should().BeNull();
            }
            if (secondPaymentStatusRequiresNewAgreement.HasValue)
            {
                viewModel.Applications.First().SecondPaymentStatus.RequiresNewEmployerAgreement = secondPaymentStatusRequiresNewAgreement.Value;
                viewModel.Applications.First().SecondPaymentStatus.ViewAgreementLink = $"{_manageApprenticeshipSiteUrl}/accounts/{_accountId}/agreements";
            }
            else
            {
                viewModel.Applications.First().SecondPaymentStatus.Should().BeNull();
            }
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

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, ApplicationsSortOrder.Ascending, ApplicationsSortField.ApprenticeName, _filter) as ViewResult;

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

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, ApplicationsSortOrder.Descending, ApplicationsSortField.ApplicationDate, _filter) as ViewResult;

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

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, ApplicationsSortOrder.Ascending, ApplicationsSortField.ApplicationDate, _filter) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            var modelApplications = viewModel.Applications.ToArray();
            modelApplications[0].ApplicationDate.Should().Be(applications[1].ApplicationDate);
            modelApplications[1].ApplicationDate.Should().Be(applications[0].ApplicationDate);
        }

        [Test]
        public async Task Then_applications_are_sorted_by_sort_field_and_then_by_name_and_ULN()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(3));   
            applications[0].ULN = 999;
            applications[0].CourseName = "Engineering";
            applications[0].FirstName = "Adam";
            applications[0].LastName = "Smith";
            applications[1].ULN = 444;
            applications[1].CourseName = "Manufacturing";
            applications[0].FirstName = "Shauna";
            applications[0].LastName = "Smith";
            applications[2].ULN = 222;
            applications[2].CourseName = "Engineering";
            applications[0].FirstName = "Adam";
            applications[0].LastName = "Smith";

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, ApplicationsSortOrder.Ascending, ApplicationsSortField.CourseName, _filter) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            var modelApplications = viewModel.Applications.ToArray();
            modelApplications[0].ULN.Should().Be(applications[2].ULN);
            modelApplications[1].ULN.Should().Be(applications[0].ULN);
            modelApplications[2].ULN.Should().Be(applications[1].ULN);
        }

        [TestCase(false, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(false, true, false, true)]
        public async Task Then_stopped_message_is_shown_when_the_apprenticeship_incentive_is_in_stopped_state(
            bool firstPaymentStatusPaymentIsStopped,
            bool secondPaymentStatusPaymentIsStopped,
            bool showStoppedMessageInFirstColumn,
            bool showStoppedMessageInSecondColumn)
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.Add(
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.FirstPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.PaymentIsStopped, firstPaymentStatusPaymentIsStopped).Create())
                .With(p => p.SecondPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.PaymentIsStopped, secondPaymentStatusPaymentIsStopped).Create())
                .Create());

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.First().FirstPaymentStatus.PaymentIsStopped.Should().Be(showStoppedMessageInFirstColumn);
            viewModel.Applications.First().SecondPaymentStatus.PaymentIsStopped.Should().Be(showStoppedMessageInSecondColumn);
        }

        [TestCase(false, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(false, true, false, true)]
        public async Task Then_withdrawn_message_is_shown_when_the_apprenticeship_incentive_is_in_withdrawn_by_compliance(
            bool firstPaymentStatusPaymentIsWithdrawn,
            bool secondPaymentStatusPaymentIsWithdrawn,
            bool showWithdrawnMessageInFirstColumn,
            bool showWithdrawnMessageInSecondColumn)
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.Add(
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.FirstPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.WithdrawnByCompliance, firstPaymentStatusPaymentIsWithdrawn).Create())
                .With(p => p.SecondPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.WithdrawnByCompliance, secondPaymentStatusPaymentIsWithdrawn).Create())
                .Create());

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.First().FirstPaymentStatus.WithdrawnByCompliance.Should().Be(showWithdrawnMessageInFirstColumn);
            viewModel.Applications.First().SecondPaymentStatus.WithdrawnByCompliance.Should().Be(showWithdrawnMessageInSecondColumn);
        }

        [TestCase(false, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(false, true, false, true)]
        public async Task Then_withdrawn_message_is_shown_when_the_apprenticeship_incentive_is_in_withdrawn_by_employer(
            bool firstPaymentStatusPaymentIsWithdrawn,
            bool secondPaymentStatusPaymentIsWithdrawn,
            bool showWithdrawnMessageInFirstColumn,
            bool showWithdrawnMessageInSecondColumn)
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.Add(
                _fixture.Build<ApprenticeApplicationModel>()
                .With(p => p.FirstPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.WithdrawnByEmployer, firstPaymentStatusPaymentIsWithdrawn).Create())
                .With(p => p.SecondPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.WithdrawnByEmployer, secondPaymentStatusPaymentIsWithdrawn).Create())
                .Create());

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.First().FirstPaymentStatus.WithdrawnByEmployer.Should().Be(showWithdrawnMessageInFirstColumn);
            viewModel.Applications.First().SecondPaymentStatus.WithdrawnByEmployer.Should().Be(showWithdrawnMessageInSecondColumn);
        }

        [Test]
        public async Task Then_employment_check_status_message_is_shown_if_employment_check_failed()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.Add(
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(p => p.FirstPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.EmploymentCheckPassed, false).Create())
                    .With(p => p.SecondPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.EmploymentCheckPassed, false).Create())
                    .Create());

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.First().FirstPaymentStatus.EmploymentCheckPassed.Should().BeFalse();
            viewModel.Applications.First().SecondPaymentStatus.EmploymentCheckPassed.Should().BeFalse();
        }
        
        [Test]
        public async Task Then_employment_check_status_message_is_not_shown_if_employment_check_result_not_set()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.Add(
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(p => p.FirstPaymentStatus, _fixture.Build<PaymentStatusModel>().Without(p => p.EmploymentCheckPassed).Create())
                    .With(p => p.SecondPaymentStatus, _fixture.Build<PaymentStatusModel>().Without(p => p.EmploymentCheckPassed).Create())
                    .Create());

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            _sut = new Web.Controllers.PaymentsController(_applicationService.Object, _legalEntitiesService.Object, _linksConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.First().FirstPaymentStatus.EmploymentCheckPassed.HasValue.Should().BeFalse();
            viewModel.Applications.First().SecondPaymentStatus.EmploymentCheckPassed.HasValue.Should().BeFalse();
        }

        [Test]
        public async Task Then_employment_check_status_message_is_not_shown_if_employment_check_passed()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.Add(
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(p => p.FirstPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.EmploymentCheckPassed, true).Create())
                    .With(p => p.SecondPaymentStatus, _fixture.Build<PaymentStatusModel>().With(p => p.EmploymentCheckPassed, true).Create())
                    .Create());

            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, _filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.First().FirstPaymentStatus.EmploymentCheckPassed.Should().BeTrue();
            viewModel.Applications.First().SecondPaymentStatus.EmploymentCheckPassed.Should().BeTrue();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("unknown")]
        public async Task Then_all_applications_should_be_displayed_when_no_filter_set_or_unknown_value(string filter)
        {
            // Arrange
            var applications = CreateApplicationListForFiltering();
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, filter) as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Applications.Count().Should().Be(applications.Count);
        }
        
        [Test]
        public async Task Then_all_applications_should_be_displayed_when_filter_set_to_all()
        {
            // Arrange
            var applications = CreateApplicationListForFiltering();
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, "All") as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Applications.Count().Should().Be(applications.Count);
        }
 
        [Test]
        public async Task Then_filtered_applications_should_be_displayed_when_filter_set_to_employer_actions()
        {
            // Arrange
            var applications = CreateApplicationListForFiltering();
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, "EmployerActions") as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Applications.Count().Should().Be(2);
        }

        [Test]
        public async Task Then_filtered_applications_should_be_displayed_when_filter_set_to_payments()
        {
            // Arrange
            var applications = CreateApplicationListForFiltering();
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, "Payments") as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Applications.Count().Should().Be(2);
        }
        
        [Test]
        public async Task Then_filtered_applications_should_be_displayed_when_filter_set_to_stopped_or_withdrawn()
        {
            // Arrange
            var applications = CreateApplicationListForFiltering();
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications };

            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(getApplicationsResponse);

            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _accountLegalEntityId } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.ListPaymentsForLegalEntity(_accountId, _accountLegalEntityId, _sortOrder, _sortField, "StoppedOrWithdrawn") as ViewResult;

            // Assert
            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Applications.Count().Should().Be(3);
        }

        private List<ApprenticeApplicationModel> CreateApplicationListForFiltering()
        {
            var applications = new List<ApprenticeApplicationModel>
            {
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(x => x.IncentiveCompleted, false)
                    .With(x => x.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(x => x.LearnerMatchFound, true)
                            .With(x => x.InLearning, true)
                            .With(x => x.HasDataLock, false)
                            .With(x => x.PaymentSent, true)
                            .With(x => x.PausePayments, false)
                            .With(x => x.EmploymentCheckPassed, true)
                            .With(x => x.WithdrawnByEmployer, false)
                            .With(x => x.WithdrawnByCompliance, false)
                            .With(x => x.PaymentIsStopped, false)
                            .With(x => x.RequiresNewEmployerAgreement, false)
                            .Create()
                    )
                    .Without(x => x.SecondPaymentStatus)
                    .Create(),
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(x => x.IncentiveCompleted, false)
                    .With(x => x.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(x => x.LearnerMatchFound, true)
                            .With(x => x.PausePayments, false)
                            .With(x => x.EmploymentCheckPassed, false)
                            .With(x => x.WithdrawnByEmployer, false)
                            .With(x => x.WithdrawnByCompliance, false)
                            .With(x => x.PaymentIsStopped, false)
                            .With(x => x.RequiresNewEmployerAgreement, false)
                            .Create()
                    )
                    .Without(x => x.SecondPaymentStatus)
                    .Create(),
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(x => x.IncentiveCompleted, false)
                    .With(x => x.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(x => x.LearnerMatchFound, true)
                            .With(x => x.InLearning, false)
                            .With(x => x.PausePayments, false)
                            .With(x => x.EmploymentCheckPassed, true)
                            .With(x => x.WithdrawnByEmployer, true)
                            .With(x => x.WithdrawnByCompliance, false)
                            .With(x => x.PaymentIsStopped, false)
                            .With(x => x.RequiresNewEmployerAgreement, false)
                            .Create()
                    )
                    .Without(x => x.SecondPaymentStatus)
                    .Create(),
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(x => x.IncentiveCompleted, false)
                    .With(x => x.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(x => x.LearnerMatchFound, true)
                            .With(x => x.InLearning, false)
                            .With(x => x.PausePayments, false)
                            .With(x => x.EmploymentCheckPassed, true)
                            .With(x => x.WithdrawnByCompliance, true)
                            .With(x => x.WithdrawnByEmployer, false)
                            .With(x => x.PaymentIsStopped, false)
                            .With(x => x.RequiresNewEmployerAgreement, false)
                            .Create()
                    ).Create(),
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(x => x.IncentiveCompleted, false)
                    .With(x => x.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(x => x.LearnerMatchFound, true)
                            .With(x => x.PausePayments, false)
                            .With(x => x.PaymentSent, false)
                            .With(x => x.PaymentSentIsEstimated, false)
                            .Without(x => x.EmploymentCheckPassed)
                            .With(x => x.WithdrawnByEmployer, false)
                            .With(x => x.WithdrawnByCompliance, false)
                            .With(x => x.PaymentIsStopped, false)
                            .With(x => x.RequiresNewEmployerAgreement, false)
                            .Create()
                    )
                    .Without(x => x.SecondPaymentStatus)
                    .Create(),
                _fixture.Build<ApprenticeApplicationModel>()
                    .With(x => x.IncentiveCompleted, false)
                    .With(x => x.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(x => x.LearnerMatchFound, true)
                            .With(x => x.PausePayments, true)
                            .With(x => x.EmploymentCheckPassed, true)
                            .With(x => x.WithdrawnByEmployer, false)
                            .With(x => x.WithdrawnByCompliance, false)
                            .With(x => x.PaymentIsStopped, true)
                            .With(x => x.RequiresNewEmployerAgreement, false)
                            .Create()
                    )
                    .Without(x => x.SecondPaymentStatus)
                    .Create(),
                 _fixture.Build<ApprenticeApplicationModel>()
                     .With(x => x.IncentiveCompleted, false)
                     .With(x => x.FirstPaymentStatus,
                        _fixture.Build<PaymentStatusModel>()
                            .With(x => x.LearnerMatchFound, true)
                            .With(x => x.RequiresNewEmployerAgreement, true)
                            .With(x => x.InLearning, true)
                            .With(x => x.HasDataLock, false)
                            .With(x => x.PaymentSent, false)
                            .With(x => x.PausePayments, false)
                            .With(x => x.EmploymentCheckPassed, true)
                            .With(x => x.WithdrawnByEmployer, false)
                            .With(x => x.WithdrawnByCompliance, false)
                            .With(x => x.PaymentIsStopped, false)
                            .Create()
                    )
                    .Without(x => x.SecondPaymentStatus)
                    .Create()
            };
            
            return applications;
        }
    }
}
