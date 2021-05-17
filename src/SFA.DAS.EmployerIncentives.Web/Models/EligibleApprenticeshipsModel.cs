using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class EligibleApprenticeshipsModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool MorePages { get; set; }
        public IEnumerable<ApprenticeshipModel> Apprenticeships { get; set; }
        public int Offset { get; set; }
        public int StartIndex { get; set; }
    }
}
