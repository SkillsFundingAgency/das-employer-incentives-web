using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public interface IUserService
    {
        Task<IEnumerable<Claim>> Claims(string userId);
    }    
}