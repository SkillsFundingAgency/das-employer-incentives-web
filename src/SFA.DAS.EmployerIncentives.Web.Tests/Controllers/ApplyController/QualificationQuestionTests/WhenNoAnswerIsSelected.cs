using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenNoAnswerIsSelected : ApplyControllerTestBase
    {
        [Test]
        public async Task Then_a_Validation_Error_Is_Displayed()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel();

            var result = await Sut.QualificationQuestion(accountId, viewModel);

            var viewResult = result as ViewResult;

            Sut.ViewData.ModelState.IsValid.Should().BeFalse();
            Sut.ViewData.ModelState.Single(x => x.Key == "HasTakenOnNewApprenticeships").Value.Errors.Should().Contain(x => x.ErrorMessage == QualificationQuestionViewModel.HasTakenOnNewApprenticeshipsNotSelectedMessage);
            viewResult.ViewName.Should().BeNullOrEmpty();
        }
    }
}
