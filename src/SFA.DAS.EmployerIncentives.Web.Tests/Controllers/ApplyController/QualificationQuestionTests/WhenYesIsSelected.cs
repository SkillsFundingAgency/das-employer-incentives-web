using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenYesIsSelected : ApplyControllerTestBase
    {
        [Test]
        public async Task Then_The_Select_Apprenticeships_Page_Is_Displayed_When_Eligible_Apprenticeships_Exist()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { HasTakenOnNewApprenticeships = true };

            var result = await Sut.QualificationQuestion(accountId, viewModel);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("SelectApprenticeships");
        }
    }
}
