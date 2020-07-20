namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public class WebConfigurationOptions
    {
        public const string EmployerIncentivesWebConfiguration = "EmployerIncentivesWeb";

        public virtual string RedisCacheConnectionString { get; set; }
        public virtual string CommitmentsBaseUrl { get; set; }
        public virtual string AllowedHashstringCharacters { get; set; }
        public virtual string Hashstring { get; set; }
    }
}
