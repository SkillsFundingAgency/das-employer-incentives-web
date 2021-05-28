using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Controllers;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.ValidateTermsSignedTests
{
    [TestFixture]
    public class WhenTermsHaveNotBeenSigned
    {
        private Mock<ILegalEntitiesService> _legalEntityServiceMock;
        private Mock<ExternalLinksConfiguration> _externalLinksConfigurationMock;
        private ApplyOrganisationController _sut;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _legalEntityServiceMock = new Mock<ILegalEntitiesService>();
            _externalLinksConfigurationMock = new Mock<ExternalLinksConfiguration>();
            var mockOptions = new Mock<IOptions<ExternalLinksConfiguration>>();
            mockOptions.Setup(x => x.Value).Returns(_externalLinksConfigurationMock.Object);

            _sut = new ApplyOrganisationController(_legalEntityServiceMock.Object, mockOptions.Object);
        }

        [Test]
        public async Task Then_The_Shutter_Page_Is_Displayed()
        {
            var accountId = "ABC123";
            var accountLegalEntityId = "ZZZZ999";
            var accountsBaseUrl = "http://test.com/";

            var legalEntity = _fixture.Build<LegalEntityModel>().With(x => x.IsAgreementSigned, false).Create();
            _legalEntityServiceMock.Setup(x => x.Get(accountId, accountLegalEntityId)).ReturnsAsync(legalEntity);

            _externalLinksConfigurationMock.Setup(x => x.ManageApprenticeshipSiteUrl).Returns(accountsBaseUrl);

            var result = await _sut.ValidateTermsSigned(accountId, accountLegalEntityId);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var viewModel = viewResult.Model as ValidateTermsSignedViewModel;
            viewModel.AccountId.Should().Be(accountId);
            viewModel.AccountsAgreementsUrl.Should().Be($"{accountsBaseUrl}/accounts/{accountId}/agreements");
            viewModel.AccountsHomeUrl.Should().Be($"{accountsBaseUrl}/accounts/{accountId}/teams");
        }
    }
}
