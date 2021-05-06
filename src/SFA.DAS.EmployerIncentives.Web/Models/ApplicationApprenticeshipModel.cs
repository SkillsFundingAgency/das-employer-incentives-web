using System;

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
        public DateTime? EmploymentStartDate { get; set; }
    }
    
}
