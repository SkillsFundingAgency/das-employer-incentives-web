using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.CannotApplyTests
{
    [TestFixture]
    public class WhenThereAreNoEligibleApprenticeships : ApplyControllerTestBase
    {
        [Test]
        public async Task Then_the_cannot_apply_page_is_displayed()
        {
            var expectedCommitmentsUrl = "expectedUrl";
            _configuration.Setup(x => x.Value.CommitmentsBaseUrl).Returns(expectedCommitmentsUrl);

            var result = await _sut.CannotApply();

            result.Model.Should().BeOfType<CannotApplyViewModel>();
            var viewModel = result.Model as CannotApplyViewModel;
            viewModel.CommitmentsUrl.Should().Be(expectedCommitmentsUrl);
            result.ViewName.Should().BeNullOrEmpty();
        }
    }
}
