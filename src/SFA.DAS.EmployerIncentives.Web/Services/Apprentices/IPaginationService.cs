using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public interface IPaginationService
    {
        Task<PagingInformation> GetPagingInformation(ApprenticesQuery apprenticesQuery);
        Task SavePagingInformation(PagingInformation pagingInformation);
    }
}
