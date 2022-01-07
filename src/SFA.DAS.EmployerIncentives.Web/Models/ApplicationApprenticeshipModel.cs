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

        private DateTime? employmentStartDate;
        public DateTime? EmploymentStartDate
        {
            get { return employmentStartDate; }
            
            set
            {
                employmentStartDate = value;
                if (employmentStartDate.HasValue)
                {
                    EmploymentStartDateDay = employmentStartDate.Value.Day;
                    EmploymentStartDateMonth = employmentStartDate.Value.Month;
                    EmploymentStartDateYear = employmentStartDate.Value.Year;
                }
            }
        }
        public string FullName => $"{FirstName} {LastName}";
        public int? EmploymentStartDateDay { get; set; }
        public int? EmploymentStartDateMonth { get; set; }
        public int? EmploymentStartDateYear { get; set; }
        public bool StartDatesAreEligible { get; set; }        
    }    
}
