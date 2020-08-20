using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Users.Types;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<Claim>> GetClaims(Guid userRef);
        Task<IEnumerable<UserModel>> Get(GetUserRequest request, CancellationToken cancellationToken = new CancellationToken());
    }
}
