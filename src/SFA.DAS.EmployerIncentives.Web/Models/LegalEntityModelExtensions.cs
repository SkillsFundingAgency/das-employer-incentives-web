using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public static class LegalEntityModelExtensions
    {
        public static IEnumerable<LegalEntityModel> ToLegalEntityModel(this IEnumerable<LegalEntityDto> dtos, IHashingService hashingService)
        {
            return dtos.Select(x => x.ToLegalEntityModel(hashingService));
        }

        public static LegalEntityModel ToLegalEntityModel(this LegalEntityDto dto, IHashingService hashingService)
        {
            return new LegalEntityModel
            {
                AccountId = hashingService.HashValue(dto.AccountId),
                AccountLegalEntityId = hashingService.HashValue(dto.AccountLegalEntityId),
                Name = dto.LegalEntityName,
                HasSignedIncentiveTerms = dto.HasSignedIncentivesTerms,
                VrfCaseStatus = dto.VrfCaseStatus,
                SignedAgreementVersion = dto.SignedAgreementVersion
            };
        }
    }
}