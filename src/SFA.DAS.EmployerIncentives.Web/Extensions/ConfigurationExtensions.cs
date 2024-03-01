using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerIncentives.Web.Extensions;

public static class ConfigurationExtensions
{
    private const string EncodingConfigKey = "SFA.DAS.Encoding";

    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());

        if (!configuration["EnvironmentName"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase))
        {
#if DEBUG
            configBuilder.AddJsonFile("appsettings.json", true);
            configBuilder.AddJsonFile("appsettings.development.json", true);
#endif
            configBuilder.AddEnvironmentVariables();

            configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                    options.ConfigurationKeysRawJsonResult = new[] { EncodingConfigKey };
                }
            );
        }

        return configBuilder.Build();
    }

    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        
        services.Configure<WebConfigurationOptions>(configuration.GetSection(WebConfigurationOptions.EmployerIncentivesWebConfiguration));
        services.Configure<WebConfigurationOptions>(configuration.GetSection(WebConfigurationOptions.EmployerIncentivesWebConfiguration));
        services.Configure<EmployerIncentivesApiOptions>(configuration.GetSection(EmployerIncentivesApiOptions.EmployerIncentivesApi));
        services.Configure<IdentityServerOptions>(configuration.GetSection(IdentityServerOptions.IdentityServerConfiguration));
        services.Configure<ExternalLinksConfiguration>(configuration.GetSection(ExternalLinksConfiguration.EmployerIncentivesExternalLinksConfiguration));
            
        var encodingConfigJson = configuration.GetSection(EncodingConfigKey).Value;
        var encodingConfig = JsonSerializer.Deserialize<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);
        
        return services;
    }
}