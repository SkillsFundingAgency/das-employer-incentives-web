using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.HashingService;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class VerificationService : IVerificationService
    {
        private readonly IBankingDetailsService _bankingDetailsService;
        private readonly IDataEncryptionService _dataEncryptionService;
        private readonly IHashingService _hashingService;
        private readonly WebConfigurationOptions _configuration;

        public VerificationService(IBankingDetailsService bankingDetailsService, IDataEncryptionService dataEncryptionService, IHashingService hashingService,
            WebConfigurationOptions configuration)
        {
            _bankingDetailsService = bankingDetailsService;
            _dataEncryptionService = dataEncryptionService;
            _hashingService = hashingService;
            _configuration = configuration;
        }

        public async Task<string> BuildAchieveServiceUrl(string hashedAccountId, Guid applicationId, string returnUrl)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var bankingDetails = await _bankingDetailsService.GetBankingDetails(accountId, applicationId);

            var data = new ApplicationInformationForExternalVerificationModel
            {
                ApplicationId = applicationId,
                IsNew = true,
                HashedAccountId = hashedAccountId,
                HashedLegalEntityId = _hashingService.HashValue(bankingDetails.LegalEntityId),
                IncentiveAmount = bankingDetails.ApplicationValue,
                VendorId = bankingDetails.VendorCode,
                SubmittedByFullName = bankingDetails.ApplicantName,
                SubmittedByEmailAddress = bankingDetails.ApplicantEmail,
            };

            var encryptedData = _dataEncryptionService.Encrypt(data.ToPsvString()).ToUrlString();

            return $"{_configuration.AchieveServiceBaseUrl}/journey=new&returnURL={returnUrl}&data={encryptedData}";
        }

    }
}