using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using SFA.DAS.EmployerIncentives.Web.Authorisation.GovUserEmployerAccount;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.Http;
using StackExchange.Redis;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Validators;
using SFA.DAS.Encoding;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyNames.HasEmployerAccount,
                    policy =>
                    {
                        policy.Requirements.Add(new EmployerAccountRequirement());
                        policy.Requirements.Add(new AccountActiveRequirement());
                        policy.RequireAuthenticatedUser();
                    });
#if DEBUG
                options.AddPolicy(
                    "StubAuthentication",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
#endif
                
            });

            return serviceCollection;
        }

        public static IServiceCollection AddEmployerAuthentication(
    this IServiceCollection serviceCollection,
    IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IAuthorizationHandler, AccountActiveAuthorizationHandler>();//TODO remove once gov login is live
            serviceCollection.AddSingleton<IStubAuthenticationService, StubAuthenticationService>();//TODO remove once gov login is live
            serviceCollection.AddSingleton<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();

            serviceCollection.Configure<GovUkOidcConfiguration>(configuration.GetSection("GovUkOidcConfiguration"));
            serviceCollection.AddAndConfigureGovUkAuthentication(configuration, typeof(EmployerAccountPostAuthenticationClaimsHandler), "","/SignIn-Stub");

            return serviceCollection;
        }


        public static IServiceCollection AddEmployerIncentivesService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEmploymentStartDateValidator, EmploymentStartDateValidator>();
            serviceCollection.AddSingleton<IEncodingService, EncodingService>();
            serviceCollection.AddSingleton<IAccountEncodingService, AccountEncodingService>();

            serviceCollection.AddClient<ILegalEntitiesService>((c, s) => new LegalEntitiesService(c, s.GetRequiredService<IAccountEncodingService>()));
            serviceCollection.AddClient<IApprenticesService>((c, s) => new ApprenticesService(c, s.GetRequiredService<IAccountEncodingService>()));
            serviceCollection.AddClient<IApplicationService>((c, s) => new ApplicationService(c, s.GetRequiredService<IAccountEncodingService>()));
            serviceCollection.AddClient<IApprenticeshipIncentiveService>((c, s) => new ApprenticeshipIncentiveService(c, s.GetRequiredService<IAccountEncodingService>()));
            serviceCollection.AddClient<IBankingDetailsService>((c, s) => new BankingDetailsService(c));
            serviceCollection.AddClient<IEmailService>((c, s) => new EmailService(c));

            serviceCollection.AddClient<ICustomClaims>((c,s) => new EmployerAccountPostAuthenticationClaimsHandler(c));

            return serviceCollection;
        }

        private static IServiceCollection AddClient<T>(
            this IServiceCollection serviceCollection,
            Func<HttpClient, IServiceProvider, T> instance) where T : class
        {
            serviceCollection.AddTransient(s =>
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

                return instance.Invoke(httpClient, s);
            });

            return serviceCollection;
        }

        public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration["EnvironmentName"] != "LOCAL" && configuration["EnvironmentName"] != "DEV")
            {
                var webConfig = configuration.GetSection(WebConfigurationOptions.EmployerIncentivesWebConfiguration).Get<WebConfigurationOptions>();

                if (webConfig != null && !string.IsNullOrEmpty(webConfig.DataProtectionKeysDatabase)
                                      && !string.IsNullOrEmpty(webConfig.RedisCacheConnectionString))
                {
                    var redisConnectionString = webConfig.RedisCacheConnectionString;
                    var dataProtectionKeysDatabase = webConfig.DataProtectionKeysDatabase;

                    var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                    services.AddDataProtection()
                        .SetApplicationName("das-employer")
                        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                }
            }
            return services;
        }

        private static Task OnRemoteFailure(
           RemoteFailureContext ctx,
           ILoggerFactory loggerFactory)
        {
            try
            {
                if (ctx.Failure.Message.Contains("Correlation failed"))
                {
                    var redirectUri = ctx.Properties.RedirectUri;
                    var logger = loggerFactory.CreateLogger("SFA.DAS.EmployerIncentives.Authentication");

                    logger.LogError(ctx.Failure, $"Correlation failed error when redirecting from {redirectUri}");
                }
            }
            catch 
            {
                // ignore errors
            }

            return Task.CompletedTask;
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
                s.GetService<IAccountEncodingService>(),
                s.GetService<ILegalEntitiesService>(),
                s.GetService<IOptions<WebConfigurationOptions>>().Value
            ));

            return serviceCollection;
        }

    }
}
