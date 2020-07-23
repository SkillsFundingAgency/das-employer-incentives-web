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
            return dtos.Select(x => new LegalEntityModel
            {
                AccountId = hashingService.HashValue(x.AccountId),
                AccountLegalEntityId = hashingService.HashValue(x.AccountLegalEntityId),
                Name = x.LegalEntityName
            });
        }
    }
}