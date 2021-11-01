using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.SelectNewApprenticeshipsTests
{
    public class WhenSelectingApprentices
    {
        private Guid _applicationId;
        private string _hashedAccountId;
        private string _hashedLegalEntityId;
        private IActionResult _result;
        private IEnumerable<ApprenticeshipModel> _apprenticeData;
        private IEnumerable<ApplicationApprenticeshipModel> _applicationApprenticeshipModel;
        private ApplicationModel _applicationModel;
        private SelectApprenticeshipsViewModel _model;
        private Mock<ILegalEntitiesService> _mockLegalEntitiesService;
        private Mock<IApprenticesService> _apprenticesServiceMock;
        private Mock<IApplicationService> _applicationServiceMock;
        private Web.Controllers.ApplyApprenticeshipsController _sut;
        private Mock<IOptions<ExternalLinksConfiguration>> _mockConfiguration;
        private string _manageApprenticeshipSiteUrl;
        private string _organisationName;

        [SetUp]
        public async Task Arrange()
        {
            _applicationId = Guid.NewGuid();
            _apprenticeData = new Fixture().CreateMany<ApprenticeshipModel>();
            _hashedAccountId = Guid.NewGuid().ToString();
            _hashedLegalEntityId = Guid.NewGuid().ToString();
            _organisationName = Guid.NewGuid().ToString();
            _manageApprenticeshipSiteUrl = $"http://{Guid.NewGuid()}";

            _applicationApprenticeshipModel = new Fixture().CreateMany<ApplicationApprenticeshipModel>();
            _applicationModel = new ApplicationModel(_applicationId, _hashedAccountId, _hashedLegalEntityId, _applicationApprenticeshipModel, false, false);
            _mockConfiguration = new Mock<IOptions<ExternalLinksConfiguration>>();

            _mockConfiguration.Setup(m => m.Value).Returns(new ExternalLinksConfiguration() { ManageApprenticeshipSiteUrl = _manageApprenticeshipSiteUrl });

            _apprenticesServiceMock = new Mock<IApprenticesService>();
            _apprenticesServiceMock
                .Setup(x => x.Get(It.Is<ApprenticesQuery>(q =>
                    q.AccountId == _hashedAccountId && q.AccountLegalEntityId == _hashedLegalEntityId)))
                .ReturnsAsync(_apprenticeData);

            _applicationServiceMock = new Mock<IApplicationService>();
            _applicationServiceMock
                .Setup(x => x.Create(_hashedAccountId, _hashedLegalEntityId, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(_applicationId);

            _applicationServiceMock
                .Setup(x => x.Get(_hashedAccountId, _applicationId, false))
                .ReturnsAsync(_applicationModel);

            _mockLegalEntitiesService = new Mock<ILegalEntitiesService>();
            _mockLegalEntitiesService.Setup(x => x.Get(_hashedAccountId, _hashedLegalEntityId)).ReturnsAsync(new LegalEntityModel() {  Name = _organisationName });

            _sut = new Web.Controllers.ApplyApprenticeshipsController(
                _apprenticesServiceMock.Object, 
                _applicationServiceMock.Object,
                _mockLegalEntitiesService.Object,
                _mockConfiguration.Object);

            _result = await _sut.SelectApprenticeships(_hashedAccountId, _hashedLegalEntityId);
            _model = ((ViewResult)_result).Model as SelectApprenticeshipsViewModel;
        }

        [Test]
        public void Then_page_title_is_set()
        {
            _model.Title.Should().Be("Which apprentices do you want to apply for?");
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
        public void Then_validation_target_control_is_set()
        {
            _model.FirstCheckboxId.Should().Be("new-apprenticeships-" + _model.Apprenticeships.First().Id);
        }

        [Test]
        public void Then_should_display_a_list_of_apprentices()
        {
            _model.Apprenticeships.Should()
                .BeEquivalentTo(_apprenticeData,
                    opt => opt
                        .Excluding(x => x.Id)
                        .Excluding(x => x.DisplayName)
                );
        }

        [Test]
        public void Then_should_have_apprentices_ordered_by_last_name()
        {
            _model.Apprenticeships.Should().BeInAscendingOrder(x => x.LastName);
        }

        [Test]
        public async Task Then_should_show_error_if_no_selection_is_made()
        {
            var request = new SelectApprenticeshipsRequest()
            {
                AccountLegalEntityId = _hashedLegalEntityId,
                AccountId = _hashedAccountId
            };
            var result = _sut.SelectApprenticeships(request);

            var viewResult = await result as ViewResult;

            viewResult.Should().NotBeNull();
            _sut.ViewData.ModelState.IsValid.Should().BeFalse();
            _sut.ViewData.ModelState.Single(x => x.Key == _model.FirstCheckboxId).Value.Errors
                .Should().Contain(x => x.ErrorMessage == SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
            viewResult?.ViewName.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task Then_the_employment_start_dates_page_is_displayed()
        {
            var request = new SelectApprenticeshipsRequest
            {
                AccountLegalEntityId = _hashedLegalEntityId,
                AccountId = _hashedAccountId,
                SelectedApprenticeships = new List<string> { _apprenticeData.First().Id }
            };

            var result = _sut.SelectApprenticeships(request);
            var redirectResult = await result as RedirectToActionResult;

            redirectResult.Should().NotBeNull();
            redirectResult?.ActionName.Should().Be("EmploymentStartDates");
            redirectResult?.ControllerName.Should().Be("ApplyEmploymentDetails");
        }

        [Test]
        public async Task Then_the_sign_new_agreement_page_is_displayed_when_a_new_agreement_needs_to_be_signed()
        {
            _applicationModel = new ApplicationModel(_applicationId, _hashedAccountId, _hashedLegalEntityId, _applicationApprenticeshipModel, false, true);

            _applicationServiceMock
                .Setup(x => x.Get(_hashedAccountId, _applicationId, false))
                .ReturnsAsync(_applicationModel);

            var request = new SelectApprenticeshipsRequest
            {
                AccountLegalEntityId = _hashedLegalEntityId,
                AccountId = _hashedAccountId,
                SelectedApprenticeships = new List<string> { _apprenticeData.First().Id }
            };

            var result = await _sut.SelectApprenticeships(request) as ViewResult;

            result.Should().NotBeNull();
            var model = result.Model as NewAgreementRequiredViewModel;
            model.AccountsAgreementsUrl.Should().Be($"{ _manageApprenticeshipSiteUrl}/accounts/{_hashedAccountId}/agreements");
            model.AccountId.Should().Be(_hashedAccountId);
            model.ApplicationId.Should().Be(_applicationId);
            model.OrganisationName.Should().Be(_organisationName);
            model.Title.Should().Be($"{_organisationName} needs to accept a new employer agreement");
        }
    }
}
