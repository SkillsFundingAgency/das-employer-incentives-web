using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenYesIsSelected : ApplyControllerTestBase
    {
        [Test]
        public async Task Then_The_Select_Apprenticeships_Page_Is_Displayed()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { HasTakenOnNewApprentices = true };

            var result = await _sut.QualificationQuestion(accountId, viewModel);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("SelectApprenticeships");
        }
    }
}
