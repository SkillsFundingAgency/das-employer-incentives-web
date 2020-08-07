namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class IncentiveApplicationApprenticeshipDto
    {
        public int ApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public decimal TotalIncentiveAmount { get; set; }
    }
}