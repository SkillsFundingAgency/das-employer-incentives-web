using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class ConfirmEmploymentDetailsRequest
    {
        public long AccountId { get; set; }
        public Guid ApplicationId { get; set; }
        public List<ApprenticeEmploymentDetailsDto> EmploymentDetails { get; set; }
    }
}
