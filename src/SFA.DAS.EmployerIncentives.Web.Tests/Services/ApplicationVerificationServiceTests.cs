using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.HashingService;
using System;
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
            var applicationId = Guid.NewGuid();
            const long legalEntityId = 120001;
            const string hashedLegalEntityId = "ABCD2X";

            var bankDetails = new BankingDetailsDto
            {
                ApplicantName = "Bob Martin",
                ApplicantEmail = "bob@clean-code.com",
                ApplicationValue = 3000,
                LegalEntityId = legalEntityId,
                VendorCode = "000000"
            };

            var bankDetailsServiceMock = new Mock<IBankingDetailsService>();
            bankDetailsServiceMock.Setup(x => x.GetBankingDetails(accountId, applicationId)).ReturnsAsync(bankDetails);

            var hashingServiceMock = new Mock<IHashingService>();
            hashingServiceMock.Setup(x => x.DecodeValue(hashedAccountId)).Returns(accountId);
            hashingServiceMock.Setup(x => x.HashValue(legalEntityId)).Returns(hashedLegalEntityId);

            var webConfigurationOptionsMock = new Mock<WebConfigurationOptions>();
            const string achieveServiceBaseUrl = "https://dfeuat.achieveservice.com/service/provide-organisation-information";
            webConfigurationOptionsMock.Setup(x => x.AchieveServiceBaseUrl).Returns(achieveServiceBaseUrl);
            var mockOptions = new Mock<IOptions<WebConfigurationOptions>>();
            mockOptions.Setup(x => x.Value).Returns(webConfigurationOptionsMock.Object);

            const string returnUrl = "employer-incentives.gov.uk/bankdetails/XFS24D/confirmed";

            const string encryptedData = "qNgwIVvU8twX0GPjF4yHcw==Qqm35mLvFQZ9RCNQ2Ff7zee2sO4CNS0H7hN9PzKM6Cfo7U+ajB52gza8VEt0F9jnKpTxYt93HWq4xZPrdzEDZw==";
            var dataEncryptionServiceMock = new Mock<IDataEncryptionService>();
            dataEncryptionServiceMock.Setup(x => x.Encrypt("ABCD2X|000000|Bob Martin|bob@clean-code.com|3000")).Returns(encryptedData);

            var expectedUrl = $"https://dfeuat.achieveservice.com/service/provide-organisation-information/journey=new&returnURL={returnUrl}&data={encryptedData.ToUrlString()}";

            var sut = new VerificationService(bankDetailsServiceMock.Object, dataEncryptionServiceMock.Object, hashingServiceMock.Object, webConfigurationOptionsMock.Object);

            // Act
            var actual = await sut.BuildAchieveServiceUrl(hashedAccountId, applicationId, returnUrl);

            // AssertW
            actual.Should().Be(expectedUrl);
        }
    }
}
