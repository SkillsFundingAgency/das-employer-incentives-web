using SFA.DAS.EmployerIncentives.Web.Services.Users.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public static class UserModelExtensions
    {
        public static IEnumerable<UserModel> ToUserModel(this IEnumerable<AccountUsers> dtos, IHashingService hashingService)
        {
            return dtos.Select(x => x.ToUserModel(hashingService));
        }

        public static UserModel ToUserModel(this AccountUsers dto, IHashingService hashingService)
        {
            return new UserModel
            {
                UserRef = dto.userRef,
                AccountId = hashingService.HashValue(dto.accountId)
            };
        }
    }
}