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
using SFA.DAS.EmployerIncentives.Web.ViewModels.Hub;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.HubController
{
    [TestFixture]
    public class WhenHubPageIsAccessed
    {
        private Web.Controllers.HubController _sut;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IApplicationService> _applicationService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;
        private List<LegalEntityModel> _legalEntities;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();

            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _legalEntities = _fixture.CreateMany<LegalEntityModel>(1).ToList();
            _legalEntities[0].AccountId = _accountId;
            _legalEntities[0].AccountLegalEntityId = _accountLegalEntityId;
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(_legalEntities);

            _applicationService = new Mock<IApplicationService>();
            var applicationsResponse = new GetApplicationsModel
            {
                BankDetailsStatus = BankDetailsStatus.Completed,
                ApprenticeApplications = _fixture.CreateMany<ApprenticeApplicationModel>(5).ToList()
            };
            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(applicationsResponse);

            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            var config = new ExternalLinksConfiguration { ManageApprenticeshipSiteUrl = "https://manage-apprentices.com" };
            _configuration.Setup(x => x.Value).Returns(config);

            _sut = new Web.Controllers.HubController(_legalEntitiesService.Object, _applicationService.Object, _configuration.Object);
        }

        [Test]
        public async Task Then_the_viewmodel_contains_details_for_the_legal_entity()
        {
            // Act
            var viewResult = await _sut.Index(_accountId, _accountLegalEntityId) as ViewResult;
            
            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HubPageViewModel;
            model.Should().NotBeNull();
            model.OrganisationName.Should().Be(_legalEntities[0].Name);
            model.HasMultipleLegalEntities.Should().BeFalse();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
        }

        [Test]
        public async Task Then_the_viewmodel_reflects_that_the_account_has_more_than_legal_entity()
        {
            // Arrange
            var secondLegalEntity = _fixture.Create<LegalEntityModel>();
            secondLegalEntity.AccountId = _accountId;
            _legalEntities.Add(secondLegalEntity);
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(_legalEntities);

            // Act
            var viewResult = await _sut.Index(_accountId, _accountLegalEntityId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HubPageViewModel;
            model.Should().NotBeNull();
            model.OrganisationName.Should().Be(_legalEntities[0].Name);
            model.HasMultipleLegalEntities.Should().BeTrue();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
        }

        [TestCase(BankDetailsStatus.NotSupplied)]
        [TestCase(BankDetailsStatus.Rejected)]
        public async Task Then_the_viewmodel_reflects_that_the_account_holder_needs_to_provide_bank_details(BankDetailsStatus status)
        {
            // Arrange
            var applicationsResponse = new GetApplicationsModel
            {
                BankDetailsStatus = status,
                ApprenticeApplications = _fixture.CreateMany<ApprenticeApplicationModel>(5).ToList()
            };
            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(applicationsResponse);

            // Act
            var viewResult = await _sut.Index(_accountId, _accountLegalEntityId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HubPageViewModel;
            model.Should().NotBeNull();
            model.OrganisationName.Should().Be(_legalEntities[0].Name);
            model.ShowBankDetailsRequired.Should().BeTrue();
            model.BankDetailsApplicationId.Should().NotBeEmpty();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
        }

        [Test]
        public async Task Then_the_viewmodel_reflects_that_the_account_holder_can_change_their_provided_bank_details()
        {
            // Arrange
            var applicationsResponse = new GetApplicationsModel
            {
                BankDetailsStatus = BankDetailsStatus.Completed,
                ApprenticeApplications = _fixture.CreateMany<ApprenticeApplicationModel>(5).ToList()
            };
            _applicationService.Setup(x => x.GetList(_accountId, _accountLegalEntityId)).ReturnsAsync(applicationsResponse);

            // Act
            var viewResult = await _sut.Index(_accountId, _accountLegalEntityId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HubPageViewModel;
            model.Should().NotBeNull();
            model.OrganisationName.Should().Be(_legalEntities[0].Name);
            model.ShowBankDetailsRequired.Should().BeFalse();
            model.BankDetailsApplicationId.Should().NotBeEmpty();
            model.ShowAmendBankDetails.Should().BeTrue();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
        }
    }
}
