using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class ApplicationApprenticeshipModel
    {
        public string ApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public decimal ExpectedAmount { get; set; }
        public long Uln { get; set; }
    }
    
}
