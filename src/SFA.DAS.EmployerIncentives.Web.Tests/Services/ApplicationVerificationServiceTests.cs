using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services
{
    public class ApplicationVerificationServiceTests
    {
        [Test]
        public async Task GivenNewApplication_WhenAgreedToEnterBankDetails_ThenBuildsEncryptedAchieveServiceUrl()
        {
            // Arrange
            const int accountId = 20001;
            const string hashedAccountId = "XFS24D"; 
            const string hashedAccountLegalEntityId = "G6M7RV";
            var applicationId = Guid.NewGuid();
            const long legalEntityId = 120001;
            const string hashedLegalEntityId = "ABCD2X";

            var bankDetails = new BankingDetailsDto
            {
                SubmittedByName = "Bob Martin",
                SubmittedByEmail = "bob@clean-code.com",
                ApplicationValue = 3000,
                LegalEntityId = legalEntityId,
                VendorCode = "000000",
                NumberOfApprenticeships = 7,
                SignedAgreements = new List<SignedAgreementDto>
                {
                    new SignedAgreementDto { SignedByEmail = "jon.skeet@google.com", SignedByName = "Jon Skeet", SignedDate = DateTime.Parse("01-09-2020 12:34:59", new CultureInfo("en-GB"))}
                }
            };

            var bankDetailsServiceMock = new Mock<IBankingDetailsService>();
            bankDetailsServiceMock.Setup(x => x.GetBankingDetails(accountId, applicationId, hashedAccountId)).ReturnsAsync(bankDetails);

            var hashingServiceMock = new Mock<IHashingService>();
            hashingServiceMock.Setup(x => x.DecodeValue(hashedAccountId)).Returns(accountId);
            
            var legalEntitiesServiceMock = new Mock<ILegalEntitiesService>();
            var legalEntity = new LegalEntityModel
            {
                AccountId = hashedAccountId,
                AccountLegalEntityId = hashedAccountLegalEntityId,
                Name = "Legal Entity",
                VrfVendorId = "ABC123",
                HashedLegalEntityId = hashedLegalEntityId
            };
            legalEntitiesServiceMock.Setup(x => x.Get(hashedAccountId, hashedAccountLegalEntityId)).ReturnsAsync(legalEntity);
            
            var webConfigurationOptionsMock = new Mock<WebConfigurationOptions>();
            const string achieveServiceBaseUrl = "https://dfeuat.achieveservice.com/service/provide-organisation-information";
            webConfigurationOptionsMock.Setup(x => x.AchieveServiceBaseUrl).Returns(achieveServiceBaseUrl);
            var mockOptions = new Mock<IOptions<WebConfigurationOptions>>();
            mockOptions.Setup(x => x.Value).Returns(webConfigurationOptionsMock.Object);

            const string returnUrl = "employer-incentives.gov.uk/completed";

            const string encryptedData = "qNgwIVvU8twX0GPjF4yHcw==Qqm35mLvFQZ9RCNQ2Ff7zee2sO4CNS0H7hN9PzKM6Cfo7U+ajB52gza8VEt0F9jnKpTxYt93HWq4xZPrdzEDZw==";
            var dataEncryptionServiceMock = new Mock<IDataEncryptionService>();
            dataEncryptionServiceMock.Setup(x => x.Encrypt(It.Is<string>(y => y.StartsWith(hashedLegalEntityId)))).Returns(encryptedData);

            var expectedUrl = $"{achieveServiceBaseUrl}?journey=new&return={returnUrl.ToUrlString()}&data={encryptedData.ToUrlString()}";

            var sut = new VerificationService(bankDetailsServiceMock.Object, dataEncryptionServiceMock.Object, hashingServiceMock.Object, legalEntitiesServiceMock.Object, webConfigurationOptionsMock.Object);

            // Act
            var actual = await sut.BuildAchieveServiceUrl(hashedAccountId, hashedAccountLegalEntityId, applicationId, returnUrl);

            // AssertW
            actual.Should().Be(expectedUrl);
        }

        [Test]
        public async Task GivenAmendApplication_WhenAgreedToEnterBankDetails_ThenBuildsEncryptedAchieveServiceUrl()
        {
            // Arrange
            const int accountId = 20001;
            const string hashedAccountId = "XFS24D";
            const string hashedAccountLegalEntityId = "G6M7RV";
            var applicationId = Guid.NewGuid();
            const long legalEntityId = 120001;
            const string hashedLegalEntityId = "ABCD2X";

            var bankDetails = new BankingDetailsDto
            {
                SubmittedByName = "Bob Martin",
                SubmittedByEmail = "bob@clean-code.com",
                ApplicationValue = 3000,
                LegalEntityId = legalEntityId,
                VendorCode = "ABC123",
                NumberOfApprenticeships = 7,
                SignedAgreements = new List<SignedAgreementDto>
                {
                    new SignedAgreementDto { SignedByEmail = "jon.skeet@google.com", SignedByName = "Jon Skeet", SignedDate = DateTime.Parse("01-09-2020 12:34:59", new CultureInfo("en-GB"))}
                }
            };

            var bankDetailsServiceMock = new Mock<IBankingDetailsService>();
            bankDetailsServiceMock.Setup(x => x.GetBankingDetails(accountId, applicationId, hashedAccountId)).ReturnsAsync(bankDetails);

            var hashingServiceMock = new Mock<IHashingService>();
            hashingServiceMock.Setup(x => x.DecodeValue(hashedAccountId)).Returns(accountId);
            hashingServiceMock.Setup(x => x.HashValue(legalEntityId)).Returns(hashedLegalEntityId);

            var legalEntitiesServiceMock = new Mock<ILegalEntitiesService>();
            var legalEntity = new LegalEntityModel
            {
                AccountId = hashedAccountId,
                AccountLegalEntityId = hashedAccountLegalEntityId,
                Name = "Legal Entity",
                VrfVendorId = "ABC123"
            };
            legalEntitiesServiceMock.Setup(x => x.Get(hashedAccountId, hashedAccountLegalEntityId)).ReturnsAsync(legalEntity);

            var webConfigurationOptionsMock = new Mock<WebConfigurationOptions>();
            const string achieveServiceBaseUrl = "https://dfeuat.achieveservice.com/service/provide-organisation-information";
            webConfigurationOptionsMock.Setup(x => x.AchieveServiceBaseUrl).Returns(achieveServiceBaseUrl);
            var mockOptions = new Mock<IOptions<WebConfigurationOptions>>();
            mockOptions.Setup(x => x.Value).Returns(webConfigurationOptionsMock.Object);

            const string returnUrl = "employer-incentives.gov.uk/hub-page";

            const string encryptedData = "qNgwIVvU8twX0GPjF4yHcw==Qqm35mLvFQZ9RCNQ2Ff7zee2sO4CNS0H7hN9PzKM6Cfo7U+ajB52gza8VEt0F9jnKpTxYt93HWq4xZPrdzEDZw==";
            var dataEncryptionServiceMock = new Mock<IDataEncryptionService>();
            dataEncryptionServiceMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(encryptedData);

            var expectedUrl = $"https://dfeuat.achieveservice.com/service/provide-organisation-information?journey=amend&return={returnUrl.ToUrlString()}&data={encryptedData.ToUrlString()}";

            var sut = new VerificationService(bankDetailsServiceMock.Object, dataEncryptionServiceMock.Object, hashingServiceMock.Object, legalEntitiesServiceMock.Object, webConfigurationOptionsMock.Object);

            // Act
            var actual = await sut.BuildAchieveServiceUrl(hashedAccountId, hashedAccountLegalEntityId, applicationId, returnUrl, amendBankDetails: true);

            // Assert
            actual.Should().Be(expectedUrl);
        }
    }
}
