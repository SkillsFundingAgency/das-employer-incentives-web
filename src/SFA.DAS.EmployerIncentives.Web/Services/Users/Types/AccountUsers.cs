using SFA.DAS.CosmosDb;
using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Users.Types
{
    public class AccountUsers : IDocument
    {
        public Guid Id { get; set; }
        public string ETag { get; set; }
        public Guid userRef { get; set; }
        public long accountId { get; set; }
        public DateTime? removed { get; set; }
        public UserRole? role { get; set; }
    }
}
