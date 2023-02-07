using SFA.DAS.EmployerIncentives.Web.Services.Users.Types;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public static class UserModelExtensions
    {
        public static IEnumerable<UserModel> ToUserModel(this IEnumerable<AccountUsers> dtos, IAccountEncodingService encodingService)
        {
            return dtos.Select(x => x.ToUserModel(encodingService));
        }

        public static UserModel ToUserModel(this AccountUsers dto, IAccountEncodingService encodingService)
        {
            return new UserModel
            {
                UserRef = dto.userRef,
                AccountId = encodingService.Encode(dto.accountId)
            };
        }
    }
}