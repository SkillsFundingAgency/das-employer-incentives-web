namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public class EmployerIncentivesWebConfiguration
    {
        public virtual string RedisCacheConnectionString { get; set; }
        public virtual string CommitmentsBaseUrl { get; set; }
        public virtual string AllowedHashstringCharacters { get; set; }
        public virtual string Hashstring { get; set; }
        public EmployerIncentivesApi EmployerIncentivesApi { get; set; }
    }
}
