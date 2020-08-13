using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerIncentives.Web.Authorization;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
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
                        policy.RequireAuthenticatedUser();
                    });

                options.AddPolicy(
                    PolicyNames.HasEmployerAccount,
                    policy =>
                    {
                        policy.RequireClaim(EmployerClaimTypes.Accounts);
                        policy.Requirements.Add(new EmployerAccountRequirement());
                    });
            });

            return serviceCollection;
        }

        public static IServiceCollection AddEmployerAuthentication(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
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
                    options.Cookie.Name = "sfa-das-employer-incentives";
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

                    //options.Events.OnTokenValidated = async (ctx) => await PopulateAccountsClaim(ctx, userService);

                    // TODO: add redirect code
                    // https://auth0.com/docs/quickstart/webapp/aspnet-core-3/01-login                    
                });

            serviceCollection.AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IUserService>((options, userService) =>
                {
                    options.Events.OnTokenValidated = async (ctx) => await PopulateAccountsClaim(ctx, userService);
                });

            return serviceCollection;
        }

        private static async Task PopulateAccountsClaim(TokenValidatedContext ctx, IUserService userService)
        {
            var userId = ctx.Principal.Claims
                .First(c => c.Type.Equals(EmployerClaimTypes.UserId))
                .Value;

            var claims = await userService.Claims(userId);

            var associatedAccountsClaim = claims.First(c => c.Type == EmployerClaimTypes.Accounts);
            ctx.Principal.Identities.First().AddClaim(associatedAccountsClaim);
        }

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
            serviceCollection.AddScoped<IAuthorizationContext, AuthorizationContext>();

            serviceCollection.AddClient<ILegalEntitiesService>((c, s) => new LegalEntitiesService(c, s.GetRequiredService<IHashingService>()));
            serviceCollection.AddClient<IApprenticesService>((c, s) => new ApprenticesService(c, s.GetRequiredService<IHashingService>()));
            serviceCollection.AddClient<IApplicationService>( (c, s) => new ApplicationService(c, s.GetRequiredService<IHashingService>()));
            serviceCollection.AddClient<IUserService>((c, s) => new UserService(c));

            return serviceCollection;
        }

        private static IServiceCollection AddClient<T>(
            this IServiceCollection serviceCollection, 
            Func<HttpClient, IServiceProvider, T> instance)  where T : class
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
    }
}
