namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public class ApprenticesQuery
    {
        public long AccountId { get; private set; }
        public long AccountLegalEntityId { get; private set; }
        
        public ApprenticesQuery(long accountId, long accountLegalEntityId)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;        
        }
    }
}
