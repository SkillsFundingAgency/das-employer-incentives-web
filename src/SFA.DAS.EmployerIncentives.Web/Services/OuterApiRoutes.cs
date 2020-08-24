using System;

namespace SFA.DAS.EmployerIncentives.Web.Services
{
    public static class OuterApiRoutes
    {
        public static string GetBankingDetailsUrl(long accountId, Guid applicationId, string hashedAccountId)
        {
            return $"accounts/{accountId}/applications/{applicationId}/bankingDetails?hashedAccountId={hashedAccountId}";
        }
    }
}