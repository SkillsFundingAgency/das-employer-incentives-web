using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services
{
    public class TestCosmosDb
    {
        public TestCosmosDb(
            IWebHostBuilder builder,
            CosmosDbConfigurationOptions cosmosDbConfigurationOptions
            )
        {
            builder
                .ConfigureServices(s =>
                {
                    s.Configure<CosmosDbConfigurationOptions>(o =>
                    {
                        o.Uri = cosmosDbConfigurationOptions.Uri;
                        o.AuthKey = cosmosDbConfigurationOptions.AuthKey;
                    });
                });
        }
    }
}
