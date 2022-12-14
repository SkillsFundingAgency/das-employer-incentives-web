using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public static class ApprenticeshipModelExtensions
    {
        public static IEnumerable<ApprenticeshipModel> ToApprenticeshipModel(this IEnumerable<ApprenticeDto> dtos, IAccountEncodingService encodingService)
        {
            return dtos.Select(x => new ApprenticeshipModel
            {
                Id = encodingService.Encode(x.ApprenticeshipId),
                LastName = x.LastName,
                FirstName = x.FirstName,
                CourseName = x.CourseName,
                StartDate = x.StartDate,
                EmploymentStartDate = x.EmploymentStartDate,
                Uln = x.Uln
            });
        }
    }
}