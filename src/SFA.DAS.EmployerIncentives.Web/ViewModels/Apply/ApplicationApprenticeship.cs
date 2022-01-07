using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class ApplicationApprenticeship
    {
        public string ApprenticeshipId { get; set; }
        public string DisplayName => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public decimal ExpectedAmount { get; set; }
        public long Uln { get; set; }
        public DateTime? EmploymentStartDate { get; set; }
        public bool StartDatesAreEligible { get; set; }
    }
}
