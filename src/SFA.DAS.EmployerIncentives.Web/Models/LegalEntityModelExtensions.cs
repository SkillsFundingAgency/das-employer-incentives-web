using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public static class LegalEntityModelExtensions
    {
        public static IEnumerable<LegalEntityModel> ToLegalEntityModel(this IEnumerable<LegalEntityDto> dtos, IAccountEncodingService encodingService)
        {
            return dtos.Select(x => x.ToLegalEntityModel(encodingService));
        }

        public static LegalEntityModel ToLegalEntityModel(this LegalEntityDto dto, IAccountEncodingService encodingService)
        {
            return new LegalEntityModel
            {
                AccountId = encodingService.Encode(dto.AccountId),
                AccountLegalEntityId = encodingService.Encode(dto.AccountLegalEntityId),
                Name = dto.LegalEntityName,
                VrfCaseStatus = dto.VrfCaseStatus,
                IsAgreementSigned = dto.IsAgreementSigned,
                VrfVendorId = dto.VrfVendorId,
                HashedLegalEntityId = dto.HashedLegalEntityId,
                BankDetailsRequired = dto.BankDetailsRequired
            };
        }
    }
}