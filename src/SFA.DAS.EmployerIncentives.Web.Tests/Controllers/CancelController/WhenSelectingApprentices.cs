﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Cancel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.CancelController.SelectNewApprenticeshipsTests
{
    public class WhenSelectingApprentices
    {
        private string _hashedAccountId;
        private string _hashedLegalEntityId;
        private string _organisationName;
        private IActionResult _result;
        private IEnumerable<ApprenticeshipIncentiveModel> _apprenticeshipIncentiveData;
        private SelectApprenticeshipsViewModel _model;
        private Mock<ILegalEntitiesService> _mockLegalEntitiesService;
        private Mock<IApprenticeshipIncentiveService> _mockApprenticeshipIncentiveService;
        private Web.Controllers.CancelController _sut;

        [SetUp]
        public async Task Arrange()
        {
            _apprenticeshipIncentiveData = new Fixture().CreateMany<ApprenticeshipIncentiveModel>();
            _hashedAccountId = Guid.NewGuid().ToString();
            _hashedLegalEntityId = Guid.NewGuid().ToString();
            _organisationName = Guid.NewGuid().ToString();

            _mockLegalEntitiesService = new Mock<ILegalEntitiesService>();
            _mockLegalEntitiesService
                .Setup(m => m.Get(_hashedAccountId, _hashedLegalEntityId))
                .ReturnsAsync(new LegalEntityModel() { Name = _organisationName });
            
            _mockApprenticeshipIncentiveService = new Mock<IApprenticeshipIncentiveService>();
            _mockApprenticeshipIncentiveService
                .Setup(m => m.GetList(_hashedAccountId, _hashedLegalEntityId))
                .ReturnsAsync(_apprenticeshipIncentiveData);

            _sut = new Web.Controllers.CancelController(_mockApprenticeshipIncentiveService.Object, _mockLegalEntitiesService.Object);

            _result = await _sut.SelectApprenticeships(_hashedAccountId, _hashedLegalEntityId);
            _model = ((ViewResult)_result).Model as SelectApprenticeshipsViewModel;
        }

        [Test]
        public void Then_page_title_is_set()
        {
            _model.Title.Should().Be("Which apprentices do you want to cancel an application for?");
        }

        [Test]
        public void Then_accountId_is_set()
        {
            _model.AccountId.Should().Be(_hashedAccountId);
        }

        [Test]
        public void Then_legalEntityId_is_set()
        {
            _model.AccountLegalEntityId.Should().Be(_hashedLegalEntityId);
        }

        [Test]
        public void Then_organisationName_is_set()
        {
            _model.OrganisationName.Should().Be(_organisationName);
        }

        [Test]
        public void Then_validation_target_control_is_set()
        {
            _model.FirstCheckboxId.Should().Be("cancel-apprenticeships-" + _model.ApprenticeshipIncentives.First().Id);
        }

        [Test]
        public void Then_should_display_a_list_of_apprentices()
        {
            _model.ApprenticeshipIncentives.Should()
                .BeEquivalentTo(_apprenticeshipIncentiveData,
                    opt => opt
                        .Excluding(x => x.Id)
                        .Excluding(x => x.DisplayName)
                );
        }

        [Test]
        public void Then_should_have_apprentices_ordered_by_last_name()
        {
            _model.ApprenticeshipIncentives.Should().BeInAscendingOrder(x => x.LastName);
        }

        [Test]
        public async Task Then_redirects_to_payments_page_when_no_apprenticeships_exist()
        {
            _mockApprenticeshipIncentiveService
               .Setup(m => m.GetList(_hashedAccountId, _hashedLegalEntityId))
               .ReturnsAsync(new List<ApprenticeshipIncentiveModel>());

            _result = await _sut.SelectApprenticeships(_hashedAccountId, _hashedLegalEntityId);
            var redirect = _result as RedirectToActionResult;

            redirect.ControllerName.Should().Be("Payments");
            redirect.ActionName.Should().Be("ListPaymentsForLegalEntity");
            redirect.RouteValues["accountId"].Should().Be(_hashedAccountId);
            redirect.RouteValues["accountLegalEntityId"].Should().Be(_hashedLegalEntityId);
        }
    }
}
