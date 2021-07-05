using AutoFixture;
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
    public class WhenSelectedApprentices
    {
        private string _hashedAccountId;
        private string _hashedAccountLegalEntityId;
        private string _organisationName;
        private IEnumerable<ApprenticeshipIncentiveModel> _apprenticeshipIncentiveData;
        private Mock<ILegalEntitiesService> _mockLegalEntitiesService;
        private Mock<IApprenticeshipIncentiveService> _mockApprenticeshipIncentiveService;
        private Web.Controllers.CancelController _sut;

        [SetUp]
        public void Arrange()
        {
            _apprenticeshipIncentiveData = new Fixture().CreateMany<ApprenticeshipIncentiveModel>();
            _hashedAccountId = Guid.NewGuid().ToString();
            _hashedAccountLegalEntityId = Guid.NewGuid().ToString();
            _organisationName = Guid.NewGuid().ToString();

            _mockLegalEntitiesService = new Mock<ILegalEntitiesService>();
            _mockLegalEntitiesService
                .Setup(m => m.Get(_hashedAccountId, _hashedAccountLegalEntityId))
                .ReturnsAsync(new LegalEntityModel() { Name = _organisationName });
            
            _mockApprenticeshipIncentiveService = new Mock<IApprenticeshipIncentiveService>();
            _mockApprenticeshipIncentiveService
                .Setup(m => m.GetList(_hashedAccountId, _hashedAccountLegalEntityId))
                .ReturnsAsync(_apprenticeshipIncentiveData);

            _sut = new Web.Controllers.CancelController(_mockApprenticeshipIncentiveService.Object, _mockLegalEntitiesService.Object);           
        }

        [Test]
        public async Task Then_add_model_error_if_no_apprenticeships_selected()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = (await _sut.SelectApprenticeships(selected)) as ViewResult;

            // assert  
            result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task Then_page_title_is_set_if_no_apprenticeships_selected()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected)).Model as SelectApprenticeshipsViewModel;

            // assert  
            result.Title.Should().Be("Which apprentices do you want to cancel an application for?");
        }

        [Test]
        public async Task Then_accountId_is_set_if_no_apprenticeships_selected()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected)).Model as SelectApprenticeshipsViewModel;

            // assert  
            result.AccountId.Should().Be(_hashedAccountId);
        }

        [Test]
        public async Task Then_legalEntityId_is_set_if_no_apprenticeships_selected()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected)).Model as SelectApprenticeshipsViewModel;

            // assert 
            result.AccountLegalEntityId.Should().Be(_hashedAccountLegalEntityId);
        }

        [Test]
        public async Task Then_organisationName_is_set_if_no_apprenticeships_selected()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected)).Model as SelectApprenticeshipsViewModel;

            // assert 
            result.OrganisationName.Should().Be(_organisationName);
        }


        [Test]
        public async Task Then_should_display_a_list_of_apprentices()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected)).Model as SelectApprenticeshipsViewModel;

            // assert 
            result.ApprenticeshipIncentives.Should()
                .BeEquivalentTo(_apprenticeshipIncentiveData,
                    opt => opt
                        .Excluding(x => x.Id)
                        .Excluding(x => x.DisplayName)
                );
        }

        [Test]
        public async Task Then_should_have_apprentices_ordered_by_last_name()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected)).Model as SelectApprenticeshipsViewModel;

            // assert 
            result.ApprenticeshipIncentives.Should().BeInAscendingOrder(x => x.LastName);
        }

        [Test]
        public async Task Then_should_show_error_if_no_selection_is_made()
        {
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected));
            var model = result.Model as SelectApprenticeshipsViewModel;

            // assert 
            model.Should().NotBeNull();
            result.ViewData.ModelState.IsValid.Should().BeFalse();
            result.ViewData.ModelState.Single(x => x.Key == model.FirstCheckboxId).Value.Errors
                .Should().Contain(x => x.ErrorMessage == (new SelectApprenticeshipsViewModel()).SelectApprenticeshipsMessage);
        }

        [Test]
        public async Task Then_displays_confirmation_page_when_apprenticeships_selected()
        {
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = _apprenticeshipIncentiveData.Select(a => a.Id).ToList()
            };

            // act
            var result = ((ViewResult)await _sut.SelectApprenticeships(selected)).Model as ConfirmApprenticeshipsViewModel;

            // assert
            result.Should().NotBeNull();
        }
    }
}
