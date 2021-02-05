using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IVerificationService
    {
        Task<string> BuildAchieveServiceUrl(string hashedAccountId, string hashedAccountLegalEntityId, Guid applicationId, string enterBankDetails, bool isNew = true);
    }
}
