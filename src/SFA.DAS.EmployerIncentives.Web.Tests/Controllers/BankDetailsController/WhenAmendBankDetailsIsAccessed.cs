using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.BankDetailsController
{
    [TestFixture]
    public class WhenAmendBankDetailsIsAccessed
    {
        private Mock<IVerificationService> _verificationService;
        private Mock<IEmailService> _emailService;
        private Mock<IApplicationService> _applicationService;
        private Mock<IHashingService> _hashingService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IOptions<ExternalLinksConfiguration>> _configuration;
        private Web.Controllers.BankDetailsController _sut;
        private Fixture _fixture;
        private string _accountId;
        private Guid _applicationId;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();

            _verificationService = new Mock<IVerificationService>();
            _emailService = new Mock<IEmailService>();
            _applicationService = new Mock<IApplicationService>();
            _hashingService = new Mock<IHashingService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _configuration = new Mock<IOptions<ExternalLinksConfiguration>>();
            _sut = new Web.Controllers.BankDetailsController(_verificationService.Object, _emailService.Object, _applicationService.Object, 
                                                             _hashingService.Object, _legalEntitiesService.Object, _configuration.Object);
        }

        [Test]
        public async Task Then_the_amend_bank_details_view_is_populated_with_the_organisation_name_and_application_details()
        {
            // Arrange
            var application = _fixture.Create<ApplicationConfirmationViewModel>();
            var legalEntity = _fixture.Create<LegalEntityModel>();
            _applicationService.Setup(x => x.Get(_accountId, _applicationId, false)).ReturnsAsync(application);
            _legalEntitiesService.Setup(x => x.Get(_accountId, application.AccountLegalEntityId)).ReturnsAsync(legalEntity);

            // Act
            var viewResult = await _sut.AmendBankDetails(_accountId, _applicationId) as ViewResult;

            // Assert
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as AmendBankDetailsViewModel;
            model.Should().NotBeNull();
            model.AccountId.Should().Be(_accountId);
            model.ApplicationId.Should().Be(_applicationId);
            model.OrganisationName.Should().Be(legalEntity.Name);
            model.Title.Should().Be($"Change {legalEntity.Name}'s bank details");
        }
    }
}
