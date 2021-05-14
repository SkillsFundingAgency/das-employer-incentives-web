using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.HomeController
{
    [TestFixture]
    public class WhenTheHomePageIsAccessed
    {
        private Web.Controllers.HomeController _sut;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Fixture _fixture;
        private string _accountId;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _sut = new Web.Controllers.HomeController(_legalEntitiesService.Object);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_the_page_redirects_to_choose_organisation()
        {
            // Act
            var result = await _sut.Home(_accountId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("GetChooseOrganisation");
            result.ControllerName.Should().Be("ApplyOrganisation");
        }
    }
}
