using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenNoAnswerIsSelected
    {
        private Web.Controllers.ApplyController _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Web.Controllers.ApplyController();
        }

        [Test]
        public async Task Then_a_Validation_Error_Is_Displayed()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel();

            var result = await _sut.QualificationQuestion(accountId, viewModel);

            var viewResult = result as ViewResult;
            var returnedViewModel = viewResult.Model as QualificationQuestionViewModel;

            returnedViewModel.Valid.Should().BeFalse();
            returnedViewModel.Errors.Should().Contain(x => x.Key == "HasTakenOnNewApprentices" && x.Value == "Select yes if you’ve taken on new apprentices that joined your payroll after 1 August 2020");
            viewResult.ViewName.Should().BeNullOrEmpty();
        }
    }
}
