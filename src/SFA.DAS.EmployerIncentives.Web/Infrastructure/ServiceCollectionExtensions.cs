using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using System;
using System.Net.Http;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHashingService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHashingService>(c =>
            {
                var settings = c.GetService<IOptions<WebConfigurationOptions>>().Value;
                return new HashingService.HashingService(settings.AllowedHashstringCharacters, settings.Hashstring);
            });

            return serviceCollection;
        }

        public static IServiceCollection AddEmployerIncentivesService(this IServiceCollection serviceCollection)
        {
            HttpClient GetOuterApiHttpClient(IServiceProvider s)
            {
                var settings = s.GetService<IOptions<EmployerIncentivesApiOptions>>().Value;

                var clientBuilder = new HttpClientBuilder()
                    .WithDefaultHeaders()
                    .WithApimAuthorisationHeader(settings)
                    .WithLogging(s.GetService<ILoggerFactory>());

                var httpClient = clientBuilder.Build();

                if (!settings.ApiBaseUrl.EndsWith("/"))
                {
                    settings.ApiBaseUrl += "/";
                }

                httpClient.BaseAddress = new Uri(settings.ApiBaseUrl);
                return httpClient;
            }

            serviceCollection.AddTransient<ILegalEntitiesService>(s => new LegalEntitiesService(GetOuterApiHttpClient(s), s.GetRequiredService<IHashingService>()));
            serviceCollection.AddTransient<IApprenticesService>(s => new ApprenticesService(GetOuterApiHttpClient(s), s.GetRequiredService<IHashingService>()));
            serviceCollection.AddTransient<IApplicationService>(s => new ApplicationService(GetOuterApiHttpClient(s), s.GetRequiredService<IHashingService>()));
            serviceCollection.AddTransient<IBankingDetailsService>(s => new BankingDetailsService(GetOuterApiHttpClient(s)));

            return serviceCollection;
        }

        public static IServiceCollection AddDataEncryptionService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDataEncryptionService>(s =>
            {
                var settings = s.GetService<IOptions<WebConfigurationOptions>>().Value;
                return new DataEncryptionService(settings.DataEncryptionServiceKey);
            });

            return serviceCollection;
        }

        public static IServiceCollection AddVerificationService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IVerificationService>(s => new VerificationService(
                s.GetService<IBankingDetailsService>(),
                s.GetService<IDataEncryptionService>(),
                s.GetService<IHashingService>(),
                s.GetService<IOptions<WebConfigurationOptions>>().Value
            ));

            return serviceCollection;
        }

    }
}
