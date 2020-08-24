using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenNoIsSelected
    {
        private ApplyQualificationController _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Web.Controllers.ApplyQualificationController();
        }

        [Test]
        public async Task Then_The_Shutter_Page_Is_Displayed()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { AccountId = accountId, HasTakenOnNewApprentices = false };

            var result = await _sut.QualificationQuestion(viewModel);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("CannotApply");
        }
    }
}
