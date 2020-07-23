using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenALegalEntityIsSelected : ApplyControllerTestBase
    {
        [Test]
        public async Task Then_the_qualification_question_is_displayed()
        {
            var result = await _sut.GetQualificationQuestion(new QualificationQuestionViewModel()) as ViewResult;

            result.Model.Should().BeOfType<QualificationQuestionViewModel>();
            result.ViewName.Should().Be("QualificationQuestion");
        }
    }
}
