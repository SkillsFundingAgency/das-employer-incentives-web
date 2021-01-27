using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public static class ApprenticeshipModelExtensions
    {
        public static IEnumerable<ApprenticeshipModel> ToApprenticeshipModel(this IEnumerable<ApprenticeDto> dtos, IHashingService hashingService)
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
    }
}