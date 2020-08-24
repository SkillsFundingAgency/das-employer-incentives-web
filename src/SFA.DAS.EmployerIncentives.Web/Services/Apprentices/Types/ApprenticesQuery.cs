namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public class ApprenticesQuery
    {
        public string AccountId { get; private set; }
        public string AccountLegalEntityId { get; private set; }
        
        public ApprenticesQuery(string accountId, string accountLegalEntityId)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;        
        }
    }
}
