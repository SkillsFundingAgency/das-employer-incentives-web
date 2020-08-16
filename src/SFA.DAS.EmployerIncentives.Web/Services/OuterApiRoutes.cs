using System;

namespace SFA.DAS.EmployerIncentives.Web.Services
{
    public static class OuterApiRoutes
    {
        public static string GetBankingDetailsUrl(in long accountId, Guid applicationId)
        {
            return $"/accounts/{accountId}/applications/{applicationId}/bankingDetails";
        }
    }
}