using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.HomeController
{
    [TestFixture]
    public class WhenApplicationStartPageIsAccessed
    {
        private Web.Controllers.HomeController _sut;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private ExternalLinksConfiguration _externalLinks;
        private string _accountId;
        private string _accountLegalEntityId;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _externalLinks = new ExternalLinksConfiguration { ManageApprenticeshipSiteUrl = "https://manage-apprentices.com" };
            _configuration.Setup(x => x.Value).Returns(_externalLinks);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<string>();

            _sut = new Web.Controllers.HomeController(_legalEntitiesService.Object, _configuration.Object);
        }

        [Test]
        public async Task Then_a_warning_message_advising_about_the_signed_legal_agreement_version_is_shown()
        {
            // Arrange
            var legalEntity = new LegalEntityModel
            {
                AccountId = _accountId,
                AccountLegalEntityId = _accountLegalEntityId,
                HasSignedIncentiveTerms = true,
                SignedAgreementVersion = 4,
                Name = "Organisation Name"
            };
            var legalEntities = new List<LegalEntityModel> { legalEntity };

            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.Start(_accountId, _accountLegalEntityId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HomeViewModel;
            model.Should().NotBeNull();
            model.NewAgreementRequired.Should().BeTrue();
            model.OrganisationName.Should().Be(legalEntity.Name);
            model.AccountsAgreementsUrl.Should().Be($"{_externalLinks.ManageApprenticeshipSiteUrl}/accounts/{_accountId}/agreements");
        }

        [Test]
        public async Task Then_a_warning_message_is_not_shown_if_the_signed_legal_agreement_version_is_the_latest_version()
        {
            // Arrange
            var legalEntity = new LegalEntityModel
            {
                AccountId = _accountId,
                AccountLegalEntityId = _accountLegalEntityId,
                HasSignedIncentiveTerms = true,
                SignedAgreementVersion = 5,
                Name = "Organisation Name"
            };
            var legalEntities = new List<LegalEntityModel> { legalEntity };

            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.Start(_accountId, _accountLegalEntityId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HomeViewModel;
            model.Should().NotBeNull();
            model.NewAgreementRequired.Should().BeFalse();
        }

        [Test]
        public async Task Then_a_warning_message_is_not_shown_if_no_legal_agreement_has_been_signed()
        {
            // Arrange
            var legalEntity = new LegalEntityModel
            {
                AccountId = _accountId,
                AccountLegalEntityId = _accountLegalEntityId,
                HasSignedIncentiveTerms = false,
                SignedAgreementVersion = null,
                Name = "Organisation Name"
            };
            var legalEntities = new List<LegalEntityModel> { legalEntity };

            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            // Act
            var result = await _sut.Start(_accountId, _accountLegalEntityId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as HomeViewModel;
            model.Should().NotBeNull();
            model.NewAgreementRequired.Should().BeFalse();
        }
    }
}
