using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerIncentives.Web.Filters;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using Microsoft.IdentityModel.Logging;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.EmployerIncentives.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
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

            _configuration = configuration;
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
            services.Configure<WebConfigurationOptions>(_configuration.GetSection(WebConfigurationOptions.EmployerIncentivesWebConfiguration));
            services.Configure<EmployerIncentivesApiOptions>(_configuration.GetSection(EmployerIncentivesApiOptions.EmployerIncentivesApi));

            //services.AddAuthorizationService();
            services.AddAuthorization<DefaultAuthorizationContextProvider>();

            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

            services.AddMvc(
                    options =>
                    {
                        options.Filters.Add(new GoogleAnalyticsFilterAttribute());
                        options.AddAuthorization();
                        options.EnableEndpointRouting = false;
                        options.SuppressOutputFormatterBuffering = true;
                    })
                .AddControllersAsServices();

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = _configuration["Environment"] == "LOCAL" ? 5001 : 443;
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
                    options.Configuration = _configuration.GetValue<string>("EmployerIncentivesWeb:SessionRedisConnectionString");
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
