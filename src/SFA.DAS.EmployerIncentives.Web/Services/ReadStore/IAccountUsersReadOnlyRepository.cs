using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerIncentives.Web.Services.Users.Types;

namespace SFA.DAS.EmployerIncentives.Web.Services.ReadStore
{
    public interface IAccountUsersReadOnlyRepository : IReadOnlyDocumentRepository<AccountUsers>
    {
    }
}
