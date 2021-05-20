using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public interface IPageTrackingService
    {
        Task<PagingInformation> GetPagingInformation(ApprenticesQuery apprenticesQuery);
        Task SavePagingInformation(PagingInformation pagingInformation);
    }
}
