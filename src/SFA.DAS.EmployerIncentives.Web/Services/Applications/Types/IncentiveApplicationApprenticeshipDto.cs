using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class IncentiveApplicationApprenticeshipDto
    {
        public long ApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public decimal TotalIncentiveAmount { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime? EmploymentStartDate { get; set; }
        public long Uln { get; set; }
    }
}