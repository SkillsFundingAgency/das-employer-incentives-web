using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Users.Types
{
    public class GetUserRequest
    {
        public Guid UserRef { get; set; }
        public UserRole Role { get; set; }        
    }
}
