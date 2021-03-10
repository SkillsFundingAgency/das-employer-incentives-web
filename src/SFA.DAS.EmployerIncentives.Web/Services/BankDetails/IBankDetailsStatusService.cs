using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Services.BankDetails
{
    public interface IBankDetailsStatusService
    {
        Task<LegalEntityModel> RecordBankDetailsComplete(string accountId, string accountLegalEntityId);
    }
}
