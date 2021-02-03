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
using NLog.Extensions.Logging;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.Email;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.ReadStore;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using SFA.DAS.EmployerIncentives.Web.Services.Users;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyNames.IsAuthenticated,
                    policy =>
                    {
                        policy.Requirements.Add(new IsAuthenticatedRequirement());
                    });

                options.AddPolicy(
                    PolicyNames.HasEmployerAccount,
                    policy =>
                    {
                        policy.Requirements.Add(new EmployerAccountRequirement());
                    });
            });

            return serviceCollection;
        }

        public static IServiceCollection AddEmployerAuthentication(
    this IServiceCollection serviceCollection,
    IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IAuthorizationHandler, IsAuthenticatedAuthorizationHandler>();
            serviceCollection.AddSingleton<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();

            _ = serviceCollection
                .AddAuthentication(options =>
                {
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = CookieNames.AuthCookie;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    var identityServerOptions = new IdentityServerOptions();
                    configuration.GetSection(IdentityServerOptions.IdentityServerConfiguration).Bind(identityServerOptions);

                    options.UsePkce = identityServerOptions.UsePkce;

                    options.ClientId = identityServerOptions.ClientId;
                    options.ClientSecret = identityServerOptions.ClientSecret;
                    options.Authority = identityServerOptions.BaseAddress;
                    options.MetadataAddress = $"{identityServerOptions.BaseAddress}/.well-known/openid-configuration";
                    options.ResponseType = OpenIdConnectResponseType.Code;

                    var scopes = identityServerOptions.Scopes.Split(' ');
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.ClaimActions.MapUniqueJsonKey("sub", "id");

                    // TODO: add redirect code ?
                    // https://auth0.com/docs/quickstart/webapp/aspnet-core-3/01-login                    
                });

            serviceCollection
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IUserService>((options, userService) =>
                {
                    options.Events.OnTokenValidated = async (ctx) => await PopulateAccountsClaim(ctx, userService);
                });

            serviceCollection
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<ILoggerFactory>((options, loggerFactory) =>
                {
                    options.Events.OnRemoteFailure = async (ctx) => await OnRemoteFailure(ctx, loggerFactory);
                });

            return serviceCollection;
        }

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
            serviceCollection.AddSingleton<IDocumentClientFactory, DocumentClientFactory>();
            serviceCollection.AddTransient<IAccountUsersReadOnlyRepository, AccountUsersReadOnlyRepository>();
            serviceCollection.AddTransient<IUserService, UserService>();

            serviceCollection.AddClient<ILegalEntitiesService>((c, s) => new LegalEntitiesService(c, s.GetRequiredService<IHashingService>()));
            serviceCollection.AddClient<IApprenticesService>((c, s) => new ApprenticesService(c, s.GetRequiredService<IHashingService>()));
            serviceCollection.AddClient<IApplicationService>((c, s) => new ApplicationService(c, s.GetRequiredService<IHashingService>()));
            serviceCollection.AddClient<IBankingDetailsService>((c, s) => new BankingDetailsService(c));
            serviceCollection.AddClient<IEmailService>((c, s) => new EmailService(c));

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
                        .SetApplicationName("das-employer-incentives-web")
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

        private static async Task PopulateAccountsClaim(
            TokenValidatedContext ctx,
            IUserService userService)
        {
            var userIdString = ctx.Principal.Claims
                .First(c => c.Type.Equals(EmployerClaimTypes.UserId))
                .Value;

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var claims = await userService.GetClaims(userId);

                claims.ToList().ForEach(c => ctx.Principal.Identities.First().AddClaim(c));
            }
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

        public static IServiceCollection AddNLog(this IServiceCollection serviceCollection)
        {
            var nLogConfiguration = new NLogConfiguration();

            serviceCollection.AddLogging((options) =>
            {
                options.AddFilter(typeof(Startup).Namespace, LogLevel.Information);
                options.SetMinimumLevel(LogLevel.Trace);
                options.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
                options.AddConsole();

                nLogConfiguration.ConfigureNLog();
            });

            return serviceCollection;
        }
    }
}
