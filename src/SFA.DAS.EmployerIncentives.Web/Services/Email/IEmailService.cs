using SFA.DAS.EmployerIncentives.Web.Services.Email.Types;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Email
{
    public interface IEmailService
    { 
        Task SendBankDetailsRequiredEmail(SendBankDetailsRequiredEmailRequest request);
    }
}
