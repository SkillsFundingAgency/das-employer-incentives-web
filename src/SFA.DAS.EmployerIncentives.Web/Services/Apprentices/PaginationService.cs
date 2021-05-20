using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.Session;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices
{
    public class PaginationService : IPaginationService
    {
        private string SessionKeyFormatString = "ApprenticeshipsPage_{0}";
        private readonly ISessionService _sessionService;

        public PaginationService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<PagingInformation> GetPagingInformation(IPaginationQuery paginationQuery)
        {
            var pagingInformation = _sessionService.Get<PagingInformation>(string.Format(SessionKeyFormatString, paginationQuery.StartIndex));
            if (pagingInformation == null)
            {
                pagingInformation = new PagingInformation
                {
                    Offset = paginationQuery.Offset,
                    StartIndex = paginationQuery.StartIndex
                };
                if (paginationQuery.StartIndex == 1)
                {
                    pagingInformation.PageNumber = 1;
                }
                else if (paginationQuery.StartIndex > 1)
                {
                    var previousStartIndex = paginationQuery.StartIndex - paginationQuery.PageSize;
                    var previousPagingInformation = _sessionService.Get<PagingInformation>(string.Format(SessionKeyFormatString, previousStartIndex));
                    if (paginationQuery.Offset == 0)
                    {
                        pagingInformation.PageNumber = previousPagingInformation.PageNumber + 1;
                    }
                    else
                    {
                        pagingInformation.PageNumber = previousPagingInformation.PageNumber;
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
