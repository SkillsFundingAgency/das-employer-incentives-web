using SFA.DAS.CosmosDb;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public class CosmosDbConfigurationOptions : ICosmosDbConfiguration
    {
        public const string CosmosDbConfiguration = "ReadStore";
        public virtual string Uri { get; set; }
        public virtual string AuthKey { get; set; }
    }
}
