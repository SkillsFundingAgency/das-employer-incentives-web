﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using System;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHashingService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHashingService>(c => {
                var settings = c.GetService<IOptions<WebConfigurationOptions>>().Value;
                return new HashingService.HashingService(settings.AllowedHashstringCharacters, settings.Hashstring);
            });

            return serviceCollection;
        }

        public static IServiceCollection AddEmployerIncentivesService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ILegalEntitiesService>(s =>
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

                return new LegalEntitiesService(httpClient, s.GetRequiredService<IHashingService>());
            });

            serviceCollection.AddTransient<IApprenticesService>(s =>
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

                return new ApprenticesService(httpClient, s.GetRequiredService<IHashingService>());
            });

            serviceCollection.AddTransient<IApplicationService>(s =>
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

                return new ApplicationService(httpClient, s.GetRequiredService<IHashingService>());
            });

            return serviceCollection;
        }
    }
}
