using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerIncentives.Web.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SFA.DAS.EmployerIncentives.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.development.json", true)
#endif
                .AddEnvironmentVariables();

            if (!configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["Environment"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }
            _configuration = config.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddOptions();
            services.Configure<EmployerIncentivesWebConfiguration>(_configuration.GetSection("EmployerIncentivesWeb"));
            services.Configure<EmployerIncentivesApi>(_configuration.GetSection("EmployerIncentivesApi"));

            var serviceProvider = services.BuildServiceProvider();

            //services.AddAuthorizationService();
            services.AddAuthorization<DefaultAuthorizationContextProvider>();

            //services.AddAndConfigureEmployerAuthentication(
            //serviceProvider.GetService<IOptions<IdentityServerConfiguration>>(),
            //serviceProvider.GetService<IEmployerAccountService>());

            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

            services.AddMvc(
                    options =>
                    {
                        options.Filters.Add(new GoogleAnalyticsFilter());
                        options.AddAuthorization();
                        options.EnableEndpointRouting = false;
                    })
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = _configuration["Environment"] == "LOCAL" ? 5001 : 443;
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            if (_configuration["Environment"] == "LOCAL" || _configuration["Environment"] == "DEV")
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _configuration["RedisCacheConnectionString"];
                });
            }

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            services.AddApplicationInsightsTelemetry();
            services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".EmployerIncentives.AntiForgery", HttpOnly = false });

            services
                .AddHashingService()
                .AddEmployerIncentivesService();

            services.AddSingleton<IDataService, MockDataService>(); // TODO

            /* if (!_environment.IsDevelopment())
            {
                services.AddHealthChecks()
                    .AddCheck<ReservationsApiHealthCheck>(
                        "Reservation Api",
                        failureStatus: HealthStatus.Unhealthy,
                        tags: new[] { "ready" })
                    .AddCheck<CommitmentsApiHealthCheck>(
                        "Commitments Api",
                        failureStatus: HealthStatus.Unhealthy,
                        tags: new[] { "ready" })
                    .AddCheck<ProviderRelationshipsApiHealthCheck>(
                        "ProviderRelationships Api",
                        failureStatus: HealthStatus.Unhealthy,
                        tags: new[] { "ready" })
                    .AddCheck<AccountApiHealthCheck>(
                        "Accounts Api",
                        failureStatus: HealthStatus.Unhealthy,
                        tags: new[] { "ready" });
            } */

            if (!_environment.IsDevelopment())
            {
                services.AddHealthChecks();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseDasHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                if (context.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.Response.Headers.Remove("X-Frame-Options");
                }

                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");

                await next();

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    var originalPath = context.Request.Path.Value;
                    context.Items["originalPath"] = originalPath;
                    context.Request.Path = "/error/404";
                    await next();
                }
            });
            app.UseAuthentication();

            if (!_environment.IsDevelopment())
            {
                app.UseHealthChecks();
            }

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
