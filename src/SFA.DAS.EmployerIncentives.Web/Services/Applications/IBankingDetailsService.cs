using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IBankingDetailsService
    {
        Task<BankingDetailsDto> GetBankingDetails(long accountId, Guid applicationId);
    }
}