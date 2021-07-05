using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class ApprenticeshipIncentiveModel
    {
        public string Id { get; set; }
        public long ApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public string DisplayName => $"{FirstName} {LastName}";
        public bool Selected { get; set; }
        public long Uln { get; set; }        
    }
}