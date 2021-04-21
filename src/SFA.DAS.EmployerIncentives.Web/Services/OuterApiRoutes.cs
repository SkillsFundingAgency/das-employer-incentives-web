using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services
{
    public static class OuterApiRoutes
    {
        public static class Application
        {
            public static string GetBankingDetailsUrl(in long accountId, Guid applicationId, in string hashedAccountId)
            {
                return $"accounts/{accountId}/applications/{applicationId}/bankingDetails?hashedAccountId={hashedAccountId}";
            }

            public static string CreateApplication(in long accountId)
            {
                return $"accounts/{accountId}/applications";
            }

            public static string GetApplication(in long accountId, Guid applicationId, bool includeApprenticeships)
            {
                return $"accounts/{accountId}/applications/{applicationId}?includeApprenticeships={includeApprenticeships}";
            }

            public static string UpdateApplication(in long accountId)
            {
                return $"accounts/{accountId}/applications";
            }

            public static string ConfirmApplication(in long accountId)
            {
                return $"accounts/{accountId}/applications";
            }

            public static string GetApplicationLegalEntity(in long accountId, Guid applicationId)
            {
                return $"accounts/{accountId}/applications/{applicationId}/accountlegalentity";
            }
        }

        public static class Apprenticeships
        {
            public static string GetApprenticeships(long accountId, long accountLegalEntityId, int pageNumber, int pageSize)
            {
                var queryParams = new Dictionary<string, string>()
                {
                    {"accountid", accountId.ToString()},
                    {"accountlegalentityid", accountLegalEntityId.ToString()},
                    {"pageNumber", pageNumber.ToString()},
                    {"pageSize", pageSize.ToString()}
                };

                return QueryHelpers.AddQueryString("apprenticeships", queryParams);
            }
        }

        public static class Email
        {
            public static string SendBankDetailsRequiredEmail()
            {
                return "email/bank-details-required";
            }

            public static string SendBankDetailsReminderEmail()
            {
                return "email/bank-details-reminder";
            }
        }

        public static class LegalEntities
        {
            public static string GetLegalEntities(in long accountId)
            {
                return $"accounts/{accountId}/legalentities";
            }

            public static string GetLegalEntity(in long accountId, in long accountLegalEntityId)
            {
                return $"accounts/{accountId}/legalentities/{accountLegalEntityId}";
            }

            public static string UpdateVrfCaseStatus(in long accountId, in long accountLegalEntityId)
            {
                return $"accounts/{accountId}/legalentity/{accountLegalEntityId}/vrfcasestatus";
           }
        }
    }
}