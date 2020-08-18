using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.HomeController
{
    [TestFixture]
    public class WhenTheSiteRootIsAccessed
    {
        private Web.Controllers.HomeController _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Web.Controllers.HomeController();
        }

        [Test]
        public async Task Then_the_caller_is_redirected_to_login()
        {
            var result = await _sut.AnonymousHome() as RedirectToActionResult;

            result.ActionName.Should().Be("login");
        }
    }
}
