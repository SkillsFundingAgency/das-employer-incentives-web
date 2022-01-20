using System.Linq;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Extensions
{
    public static class ApprenticeApplicationModelExtensions
    {
        public static IQueryable<ApprenticeApplicationModel> Sort(this IQueryable<ApprenticeApplicationModel> applications, string sortOrder, string sortField)
        {
            if (sortOrder == ApplicationsSortOrder.Descending)
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    applications = applications.OrderByDescending(sortField).ThenBy(x => x.ULN).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    applications = applications.OrderByDescending(sortField).ThenBy(x => x.ULN);
                }
            }
            else
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    applications = applications.OrderBy(sortField).ThenBy(x => x.ULN).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    applications = applications.OrderBy(sortField).ThenBy(x => x.ULN);
                }
            }

            return applications;
        }
    }
}
