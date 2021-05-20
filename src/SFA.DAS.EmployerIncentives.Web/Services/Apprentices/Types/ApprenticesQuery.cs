namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public class ApprenticesQuery : IPaginationQuery
    {
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public int PageSize { get; }
        public int Offset { get; }
        public int StartIndex { get; }
        
        public ApprenticesQuery(string accountId, string accountLegalEntityId, int pageSize, int offset, int startIndex)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            PageSize = pageSize;
            Offset = offset;
            StartIndex = startIndex;
        }
    }
}
