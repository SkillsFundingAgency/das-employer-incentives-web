using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.HomeController
{
    [TestFixture]
    public class WhenTheSiteRootIsAccessed
    {
        private Web.Controllers.HomeController _sut;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _sut = new Web.Controllers.HomeController(_legalEntitiesService.Object, _configuration.Object, null, null);
        }

        [Test]
        public async Task Then_the_caller_is_redirected_to_login()
        {
            var result = await _sut.AnonymousHome() as RedirectToActionResult;

            result.ActionName.Should().Be("login");
        }
    }
}
