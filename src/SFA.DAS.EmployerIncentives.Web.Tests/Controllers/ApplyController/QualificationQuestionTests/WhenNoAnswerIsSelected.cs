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
            var viewModel = new QualificationQuestionViewModel() { AccountId = accountId };

            var result = await Sut.QualificationQuestion(viewModel);

            var viewResult = result as ViewResult;

            Sut.ViewData.ModelState.IsValid.Should().BeFalse();
            Sut.ViewData.ModelState.Single(x => x.Key == "HasTakenOnNewApprentices").Value.Errors.Should().Contain(x => x.ErrorMessage == QualificationQuestionViewModel.HasTakenOnNewApprenticesNotSelectedMessage);
            viewResult.ViewName.Should().BeNullOrEmpty();
        }
    }
}
