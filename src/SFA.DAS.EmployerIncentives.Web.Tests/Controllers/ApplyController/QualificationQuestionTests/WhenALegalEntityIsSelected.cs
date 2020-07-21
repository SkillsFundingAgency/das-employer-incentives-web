using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenALegalEntityIsSelected : ApplyControllerTestBase
    {
        [Test]
        public async Task Then_the_qualification_question_is_displayed()
        {
            var result = (ViewResult)await Sut.QualificationQuestion();

            result.Model.Should().BeOfType<QualificationQuestionViewModel>();
            result.ViewName.Should().BeNullOrEmpty();
        }
    }
}
