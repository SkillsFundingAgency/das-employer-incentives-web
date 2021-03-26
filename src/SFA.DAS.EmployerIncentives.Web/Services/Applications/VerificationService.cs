using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.HashingService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class VerificationService : IVerificationService
    {
        private readonly IBankingDetailsService _bankingDetailsService;
        private readonly IDataEncryptionService _dataEncryptionService;
        private readonly IHashingService _hashingService;
        private readonly ILegalEntitiesService _legalEntitiesService;
        private readonly WebConfigurationOptions _configuration;

        public VerificationService(IBankingDetailsService bankingDetailsService, IDataEncryptionService dataEncryptionService, IHashingService hashingService,
            ILegalEntitiesService legalEntitiesService, WebConfigurationOptions configuration)
        {
            _bankingDetailsService = bankingDetailsService;
            _dataEncryptionService = dataEncryptionService;
            _hashingService = hashingService;
            _legalEntitiesService = legalEntitiesService;
            _configuration = configuration;
        }

        public async Task<string> BuildAchieveServiceUrl(string hashedAccountId, string hashedAccountLegalEntityId, Guid applicationId, string returnUrl, bool amendBankDetails = false)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var bankingDetails = await _bankingDetailsService.GetBankingDetails(accountId, applicationId, hashedAccountId);

            if (bankingDetails == null) throw new ArgumentException("Requested banking details records cannot be found");
            if (bankingDetails.SignedAgreements == null || !bankingDetails.SignedAgreements.Any()) throw new ArgumentException("Requested application records are invalid");
            
            var legalEntity = await _legalEntitiesService.Get(hashedAccountId, hashedAccountLegalEntityId);

            var data = new ApplicationInformationForExternalVerificationModel
            {
                ApplicationId = applicationId,
                LegalEntityName = legalEntity.Name,
                IsNew = !amendBankDetails,
                HashedAccountId = hashedAccountId,
                HashedLegalEntityId = _hashingService.HashValue(bankingDetails.LegalEntityId),
                IncentiveAmount = bankingDetails.ApplicationValue,
                VendorId = bankingDetails.VendorCode,
                SubmittedByFullName = bankingDetails.SubmittedByName,
                SubmittedByEmailAddress = bankingDetails.SubmittedByEmail,
                NumberOfApprenticeships = bankingDetails.NumberOfApprenticeships,
                SignedAgreements = bankingDetails.SignedAgreements?.Select(x => new SignedAgreementModel
                {
                    SignedByEmail = x.SignedByEmail,
                    SignedByName = x.SignedByName,
                    SignedDate = x.SignedDate
                })
            };

            if (amendBankDetails)
            {
                data.VendorId = legalEntity.VrfVendorId;
                data.LegalEntityName = legalEntity.Name;
            }

            var encryptedData = _dataEncryptionService.Encrypt(data.ToPsvString()).ToUrlString();

            var journeyType = "new";
            if (amendBankDetails)
            {
                journeyType = "amend";
            }

            return $"{_configuration.AchieveServiceBaseUrl}?journey={journeyType}&return={returnUrl.ToUrlString()}&data={encryptedData}";
        }

    }
}