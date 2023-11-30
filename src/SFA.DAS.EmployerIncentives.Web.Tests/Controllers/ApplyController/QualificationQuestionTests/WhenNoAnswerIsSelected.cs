using System.Linq;
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
    public class WhenNoAnswerIsSelected
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
        public void Then_a_Validation_Error_Is_Displayed()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { AccountId = accountId };

            var result = _sut.QualificationQuestion(viewModel);

            var viewResult = result as ViewResult;

            _sut.ViewData.ModelState.IsValid.Should().BeFalse();
            _sut.ViewData.ModelState.Single(x => x.Key == "HasTakenOnNewApprentices").Value.Errors.Should().Contain(x => x.ErrorMessage == QualificationQuestionViewModel.HasTakenOnNewApprenticesNotSelectedMessage);
            viewResult.ViewName.Should().BeNullOrEmpty();
        }
    }
}
