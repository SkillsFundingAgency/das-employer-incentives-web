using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.ChooseOrganisationTests
{
    [TestFixture]
    public class WhenAccountHasMultipleLegalEntities
    {
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private ApplyOrganisationController _sut;
        private Fixture _fixture;
        private string _accountId;
        private ChooseOrganisationViewModel _viewModel;
        private ExternalLinksConfiguration _config;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _viewModel = _fixture.Create<ChooseOrganisationViewModel>();
            _viewModel.AccountId = _accountId;
            _config = _fixture.Create<ExternalLinksConfiguration>();
            _configuration.Setup(x => x.Value).Returns(_config);

            _sut = new ApplyOrganisationController(_legalEntitiesService.Object, _configuration.Object);
        }

        [Test]
        public async Task Then_the_choose_organisation_page_is_displayed_for_the_account()
        {
            // Arrange
            var legalEntities = _fixture.CreateMany<LegalEntityModel>(2).ToList();
            legalEntities[0].AccountId = _accountId;
            legalEntities[1].AccountId = _accountId;
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.GetChooseOrganisation(_viewModel) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as ChooseOrganisationViewModel;
            model.AccountHomeUrl.Should().Be($"{_config.ManageApprenticeshipSiteUrl}/accounts/{_accountId}/teams");
            model.AccountId.Should().Be(_accountId);
            model.Organisations.FirstOrDefault(x => x.AccountLegalEntityId == legalEntities[0].AccountLegalEntityId).Should().NotBeNull();
            model.Organisations.FirstOrDefault(x => x.AccountLegalEntityId == legalEntities[1].AccountLegalEntityId).Should().NotBeNull();
        }
    }
}
