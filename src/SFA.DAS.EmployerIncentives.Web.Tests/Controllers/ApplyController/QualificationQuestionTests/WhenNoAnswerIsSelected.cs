using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

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

            var result = await _sut.QualificationQuestion(accountId, viewModel);

            var viewResult = result as ViewResult;

            _sut.ViewData.ModelState.IsValid.Should().BeFalse();
            _sut.ViewData.ModelState.Single(x => x.Key == "HasTakenOnNewApprentices").Value.Errors.Should().Contain(x => x.ErrorMessage == QualificationQuestionViewModel.HasTakenOnNewApprenticesNotSelectedMessage);
            viewResult.ViewName.Should().BeNullOrEmpty();
        }
    }
}
