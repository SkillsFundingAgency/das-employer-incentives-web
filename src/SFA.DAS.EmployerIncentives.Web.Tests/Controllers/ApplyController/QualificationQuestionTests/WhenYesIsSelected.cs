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
        public async Task Then_The_Choose_Organisation_Page_Is_Displayed_When_Eligible_Apprenticeships_Exist()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { AccountId = accountId, HasTakenOnNewApprentices = true };

            var result = await Sut.QualificationQuestion(viewModel);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("SelectApprenticeships");
        }
    }
}
