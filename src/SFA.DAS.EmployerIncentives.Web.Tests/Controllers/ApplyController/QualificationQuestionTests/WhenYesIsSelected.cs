using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenYesIsSelected
    {
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private ApplyQualificationController _sut;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _sut = new Web.Controllers.ApplyQualificationController(_legalEntitiesService.Object);
        }


        [Test]
        public async Task Then_The_Agreement_Is_Checked_When_Eligible_Apprenticeships_Exist()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { AccountId = accountId, HasTakenOnNewApprentices = true };

            var result = await _sut.QualificationQuestion(viewModel);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("ValidateTermsSigned");
            redirectResult.ControllerName.Should().Be("ApplyOrganisation");
        }
    }
}
