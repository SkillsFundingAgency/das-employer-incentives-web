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
    public class WhenAccountHasASingleLegalEntity
    {
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private ApplyOrganisationController _sut;
        private Fixture _fixture;
        private string _accountId;
        private ChooseOrganisationViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _sut = new ApplyOrganisationController(_legalEntitiesService.Object, _configuration.Object);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _viewModel = _fixture.Create<ChooseOrganisationViewModel>();
            _viewModel.AccountId = _accountId;           
        }

        [Test]
        public async Task Then_the_hub_page_is_displayed_for_the_account()
        {
            // Arrange
            var legalEntities = _fixture.CreateMany<LegalEntityModel>(1).ToList();
            legalEntities[0].AccountId = _accountId;
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.GetChooseOrganisation(_viewModel) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Hub");
        }
    }
}
