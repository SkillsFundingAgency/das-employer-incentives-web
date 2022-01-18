namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public class WebConfigurationOptions
    {
        public const string EmployerIncentivesWebConfiguration = "EmployerIncentivesWeb";
        public virtual string RedisCacheConnectionString { get; set; }
        public virtual string DataProtectionKeysDatabase { get; set; }
        public virtual string AllowedHashstringCharacters { get; set; }
        public virtual string Hashstring { get; set; }
        public virtual string ZenDeskSnippetKey { get; set; }
        public virtual string ZenDeskSectionId { get; set; }
        public virtual string ZenDeskCobrowsingSnippetKey { get; set; }
        public virtual string AchieveServiceBaseUrl { get; set; }
        public virtual string DataEncryptionServiceKey { get; set; }
        public virtual string ApplicationShutterPageDate { get; set; }
        public virtual bool DisplayEmploymentCheckResult { get; set; }
    }
}
