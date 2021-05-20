using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Session;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public class PageTrackingService : IPageTrackingService
    {
        private string SessionKeyFormatString = $"ApprenticeshipsPage_{0}";
        private readonly ISessionService _sessionService;

        public PageTrackingService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<PagingInformation> GetPagingInformation(ApprenticesQuery apprenticesQuery)
        {
            var pagingInformation = _sessionService.Get<PagingInformation>(string.Format(SessionKeyFormatString, apprenticesQuery.StartIndex));
            if (pagingInformation == null)
            {
                pagingInformation = new PagingInformation
                {
                    Offset = apprenticesQuery.Offset,
                    StartIndex = apprenticesQuery.StartIndex
                };
                if (apprenticesQuery.StartIndex == 1)
                {
                    pagingInformation.PageNumber = 1;
                }
                else if (apprenticesQuery.StartIndex > 1)
                {
                    var previousStartIndex = apprenticesQuery.StartIndex - apprenticesQuery.PageSize;
                    pagingInformation = _sessionService.Get<PagingInformation>(string.Format(SessionKeyFormatString, previousStartIndex));
                    if (apprenticesQuery.Offset == 0)
                    {
                        pagingInformation.PageNumber++;
                    }
                }

                await SavePagingInformation(pagingInformation);
            }
            return pagingInformation;
        }

        public async Task SavePagingInformation(PagingInformation pagingInformation)
        {
            _sessionService.Set(string.Format(SessionKeyFormatString, pagingInformation.StartIndex), pagingInformation);
        }
    }
}
