using System.Threading.Tasks;
using AutoFixture;
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
    public class WhenALegalEntityIsSelected
    {
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private ApplyQualificationController _sut;
        private Fixture _fixture;
        private string _accountId;
        private string _accountLegalEntityId;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _sut = new Web.Controllers.ApplyQualificationController(_legalEntitiesService.Object);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_the_qualification_question_is_displayed()
        {
            var result = await _sut.GetQualificationQuestion(_accountId, _accountLegalEntityId) as ViewResult;

            result.Model.Should().BeOfType<QualificationQuestionViewModel>();
            result.ViewName.Should().Be("QualificationQuestion");
        }
    }
}
