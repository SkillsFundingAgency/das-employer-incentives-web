using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public class EmployerIncentivesApi : IApimClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string SubscriptionKey { get; set; }
        public string ApiVersion { get; set; }
    }
}
