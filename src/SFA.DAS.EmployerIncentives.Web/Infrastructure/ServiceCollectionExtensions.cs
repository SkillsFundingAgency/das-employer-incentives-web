using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{

    public class TestOpenIdConnectProtocolValidator : OpenIdConnectProtocolValidator
    {
        public TestOpenIdConnectProtocolValidator()
        {
            RequireState = false;
        }

        public override void ValidateAuthenticationResponse(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateAuthenticationResponse(validationContext);
        }
        protected override void ValidateNonce(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateNonce(validationContext);
        }

        protected override void ValidateAtHash(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateAtHash(validationContext);
        }

        protected override void ValidateCHash(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateCHash(validationContext);
        }

        protected override void ValidateIdToken(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateIdToken(validationContext);
        }

        protected override void ValidateState(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateState(validationContext);
        }

        public override void ValidateTokenResponse(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateTokenResponse(validationContext);
        }

        public override void ValidateUserInfoResponse(OpenIdConnectProtocolValidationContext validationContext)
        {
            base.ValidateUserInfoResponse(validationContext);
        }
    }
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
                        policy.RequireAuthenticatedUser();
                        //policy.RequireClaim(EmployerClaims.AccountsClaimsTypeIdentifier);
                    });
            });

            return serviceCollection;
        }

        public static IServiceCollection AddEmployerAuthentication(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection
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
                    options.Cookie.Name = "SFA.DAS.EmployerIncentives.Web.Auth";
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    var identityServerOptions = new IdentityServerOptions();
                    configuration.GetSection(IdentityServerOptions.IdentityServerConfiguration).Bind(identityServerOptions);
                    
                    options.ClientId = identityServerOptions.ClientId;
                    options.ClientSecret = identityServerOptions.ClientSecret;
                    options.Authority = identityServerOptions.BaseAddress;
                    options.MetadataAddress = $"{identityServerOptions.BaseAddress}/.well-known/openid-configuration";                    
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    
                    options.ProtocolValidator = new TestOpenIdConnectProtocolValidator();
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

                    //Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectHandler.RedeemAuthorizationCodeAsync(

                    var scopes = identityServerOptions.Scopes.Split(' ');
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.ClaimActions.MapUniqueJsonKey("sub", "id");

                    options.Events.OnTokenValidated = async (ctx) => await PopulateAccountsClaim(ctx);

                    options.Events.OnAccessDenied = (ctx) =>
                    {
                        var temp = ctx.AccessDeniedPath;
                        return Task.CompletedTask;
                    };

                    options.Events.OnAuthenticationFailed = (ctx) =>
                    {
                        var temp = ctx.Exception;
                        return Task.CompletedTask;
                    };

                    options.Events.OnAuthorizationCodeReceived = (ctx) =>
                    {
                        return Task.CompletedTask;

                        // this code generates the same error
                        //try
                        //{
                        //    var credential = new ClientCredential(ctx.Options.ClientId, ctx.Options.ClientSecret);
                        //    //var authContext = new AuthenticationContext(ctx.Options.Authority);
                        //    //var authContext = new AuthenticationContext(@"https://login.microsoftonline.com/citizenazuresfabisgov.onmicrosoft.com");
                        //    var authContext = new AuthenticationContext(@"https://login.microsoftonline.com/1a92889b-8ea1-4a16-8132-347814051567/v2.0");
                            
                        //    var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(ctx.TokenEndpointRequest.Code,
                        //        new Uri(ctx.TokenEndpointRequest.RedirectUri, UriKind.RelativeOrAbsolute), credential, ctx.Options.Resource);
                        //    ctx.HandleCodeRedemption(authResult.AccessToken, ctx.ProtocolMessage.IdToken);
                        //}
                        //catch(Exception ex)
                        //{
                        //    var temp = ex.Message;
                        //}                        
                        
                    };

                    options.Events.OnMessageReceived = (ctx) =>
                    {                        
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToIdentityProvider = (ctx) =>
                    {                        
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToIdentityProviderForSignOut = (ctx) =>
                    {
                        // TODO: add redirect code
                        // https://auth0.com/docs/quickstart/webapp/aspnet-core-3/01-login
                        return Task.CompletedTask;
                    };

                    options.Events.OnRemoteFailure = (ctx) =>
                    {
                        return Task.CompletedTask;
                    };

                    options.Events.OnRemoteSignOut = (ctx) =>
                    {
                        return Task.CompletedTask;
                    };

                    options.Events.OnSignedOutCallbackRedirect = (ctx) =>
                    {
                        return Task.CompletedTask;
                    };

                    options.Events.OnTicketReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    };

                    options.Events.OnTokenResponseReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    };

                    options.Events.OnUserInformationReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    };
                });                

            return serviceCollection;
        }

        private static Task PopulateAccountsClaim(TokenValidatedContext ctx) //, IEmployerAccountService accountsSvc)
        {
            var userId = ctx.Principal.Claims
                .First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier))
                .Value;

            return Task.CompletedTask;
            //var associatedAccountsClaim = await accountsSvc.GetClaim(userId, EmployerClaims.AccountsClaimsTypeIdentifier);
            //ctx.Principal.Identities.First().AddClaim(associatedAccountsClaim);
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
