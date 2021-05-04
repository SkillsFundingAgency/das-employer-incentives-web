namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public class ApprenticesQuery
    {
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int Offset { get; }
        
        public ApprenticesQuery(string accountId, string accountLegalEntityId, int pageNumber, int pageSize, int offset)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Offset = offset;
        }
    }
}
