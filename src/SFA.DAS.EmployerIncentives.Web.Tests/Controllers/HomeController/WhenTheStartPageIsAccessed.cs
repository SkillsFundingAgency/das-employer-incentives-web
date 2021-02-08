using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using AutoFixture;
using SFA.DAS.EmployerIncentives.Web.Models;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.HomeController
{
    [TestFixture]
    public class WhenTheStartPageIsAccessed
    {
        private Web.Controllers.HomeController _sut;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            var config = new ExternalLinksConfiguration { ManageApprenticeshipSiteUrl = "Https://manage-apprenticeships.com" };
            _configuration.Setup(x => x.Value).Returns(config);
            _sut = new Web.Controllers.HomeController(_legalEntitiesService.Object, _configuration.Object);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_the_view_model_is_populated_accordingly_if_the_account_has_multiple_legal_entities()
        {
            // Arrange
            var legalEntities = _fixture.CreateMany<LegalEntityModel>(3).ToList();
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var viewResult = await _sut.Start(_accountId, _accountLegalEntityId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HomeViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
            model.HasMultipleLegalEntities.Should().BeTrue();
        }

        [Test]
        public async Task Then_the_view_model_is_populated_accordingly_if_the_account_has_a_single_legal_entity()
        {
            // Arrange
            var legalEntities = _fixture.CreateMany<LegalEntityModel>(1).ToList();
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var viewResult = await _sut.Start(_accountId, _accountLegalEntityId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HomeViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
            model.HasMultipleLegalEntities.Should().BeFalse();
        }
    }
}
