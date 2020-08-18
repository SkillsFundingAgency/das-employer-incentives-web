namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public sealed class ExternalLinksConfiguration
    {
        public const string EmployerIncentivesExternalLinksConfiguration = "ExternalLinks";

        public string ManageApprenticeshipSiteUrl { get; set; }
        public string CommitmentsSiteUrl { get; set; }
        public string EmployerRecruitmentSiteUrl { get; set; }
    }
}
