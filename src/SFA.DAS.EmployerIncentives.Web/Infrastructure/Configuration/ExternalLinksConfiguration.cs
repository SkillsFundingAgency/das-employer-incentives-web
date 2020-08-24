namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public class ExternalLinksConfiguration
    {
        public const string EmployerIncentivesExternalLinksConfiguration = "ExternalLinks";

        public virtual string ManageApprenticeshipSiteUrl { get; set; }
        public virtual string CommitmentsSiteUrl { get; set; }
        public virtual string EmployerRecruitmentSiteUrl { get; set; }
    }
}
