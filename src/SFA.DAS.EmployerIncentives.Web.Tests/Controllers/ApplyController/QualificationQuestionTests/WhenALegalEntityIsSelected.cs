using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenALegalEntityIsSelected
    {
        private Web.Controllers.ApplyController _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Web.Controllers.ApplyController();
        }

        [Test]
        public async Task Then_the_qualification_question_is_displayed()
        {
            var result = await _sut.QualificationQuestion();

            result.Model.Should().BeOfType<QualificationQuestionViewModel>();
            result.ViewName.Should().BeNullOrEmpty();
        }
    }
}
