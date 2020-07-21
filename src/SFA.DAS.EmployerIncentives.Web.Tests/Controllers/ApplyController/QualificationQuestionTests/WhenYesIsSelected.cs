using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
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
            ApprenticesServiceMock
                .Setup(m => m.Get(It.IsAny<ApprenticesQuery>()))
                .ReturnsAsync(Fixture.CreateMany<ApprenticeDto>());

            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { HasTakenOnNewApprentices = true };

            var result = await Sut.QualificationQuestion(accountId, viewModel);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("SelectApprenticeships");
        }
    }
}
