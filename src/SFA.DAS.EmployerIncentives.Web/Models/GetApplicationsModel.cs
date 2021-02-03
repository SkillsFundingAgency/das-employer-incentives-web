using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class GetApplicationsModel
    {
        public IEnumerable<ApprenticeApplicationModel> ApprenticeApplications { get; set; }
        public BankDetailsStatus BankDetailsStatus { get; set; }
        public Guid? FirstSubmittedApplicationId { get; set; }
    }
}
