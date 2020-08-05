using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.ValidateTermsSignedTests
{
    [TestFixture]
    public class WhenTermsHaveBeenSigned
    {
        private Mock<ILegalEntitiesService> _legalEntityServiceMock;
        private ApplyOrganisationController _sut;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _legalEntityServiceMock = new Mock<ILegalEntitiesService>();

            _sut = new ApplyOrganisationController(_legalEntityServiceMock.Object, Mock.Of<WebConfigurationOptions>());
        }

        [Test]
        public async Task Then_The_Select_Apprenticeships_Page_Is_Displayed()
        {
            var accountId = "ABC123";
            var accountLegalEntityId = "ZZZZ999";

            var legalEntity = _fixture.Build<LegalEntityModel>().With(x => x.HasSignedIncentiveTerms, true).Create();
            _legalEntityServiceMock.Setup(x => x.Get(accountId, accountLegalEntityId)).ReturnsAsync(legalEntity);

            var result = await _sut.ValidateTermsSigned(accountId, accountLegalEntityId);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("SelectApprenticeships");
            redirectResult.ControllerName.Should().Be("ApplyApprenticeships");
        }
    }
}
