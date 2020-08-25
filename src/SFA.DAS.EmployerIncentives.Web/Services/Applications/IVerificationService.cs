using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IVerificationService
    {
        Task<string> BuildAchieveServiceUrl(string hashedAccountId, Guid applicationId, string enterBankDetails);
    }
}
