namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class ApprenticeshipModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public string DisplayName => $"{FirstName} {LastName}";
    }
}