using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public static class ApprenticeshipModelExtensions
    {
        public static IEnumerable<ApprenticeshipModel> ToApprenticeshipModel(this IEnumerable<ApprenticeDto> dtos,
            IHashingService hashingService)
        {
            return dtos.Select(x => new ApprenticeshipModel
            {
                Id = hashingService.HashValue(x.ApprenticeshipId),
                LastName = x.LastName,
                FirstName = x.FirstName,
                CourseName = x.CourseName,
                StartDate = x.StartDate,
                Uln = x.Uln
            });
        }

        public static EligibleApprenticeshipsModel ToEligibleApprenticeshipsModel(this EligibleApprenticesDto dto, IHashingService hashingService)
        {
            return new EligibleApprenticeshipsModel
            {
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                MorePages = dto.MorePages,
                Apprenticeships = dto.Apprenticeships.ToApprenticeshipModel(hashingService),
                Offset = dto.Offset,
                StartIndex = dto.StartIndex
            };
        }
    }
}