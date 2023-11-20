using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using SFA.DAS.EmployerIncentives.Web.Extensions;
using SFA.DAS.EmployerIncentives.Web.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;

namespace SFA.DAS.EmployerIncentives.Web;

[ExcludeFromCodeCoverage]
public class Startup
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _environment = environment;
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });
            
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddConfigurationOptions(_configuration);
            
        services.AddAuthorizationPolicies();
        services.AddAuthorization<DefaultAuthorizationContextProvider>();
        services.AddEmployerAuthentication(_configuration);

        services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

        if (_configuration[$"{WebConfigurationOptions.EmployerIncentivesWebConfiguration}:UseGovSignIn"] != null
            && _configuration[$"{WebConfigurationOptions.EmployerIncentivesWebConfiguration}:UseGovSignIn"]
                .Equals("true", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddMaMenuConfiguration("signout",  _configuration["ResourceEnvironmentName"]);
        }
        else
        {
            var identityServerOptions = new IdentityServerOptions();
            _configuration.GetSection(IdentityServerOptions.IdentityServerConfiguration).Bind(identityServerOptions);
            services.AddMaMenuConfiguration("signout", identityServerOptions.ClientId, _configuration["ResourceEnvironmentName"]);
        }

        services.AddMvc(
                options =>
                {
                    if (!_configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                    {
                        options.Filters.Add(new AuthorizeFilter(PolicyNames.HasEmployerAccount));
                    }

                    options.Filters.Add(new ApplicationShutterFilterAttribute(_configuration));
                    options.Filters.Add(new GoogleAnalyticsFilterAttribute());
                    options.EnableEndpointRouting = false;
                    options.SuppressOutputFormatterBuffering = true;                        
                })
            .SetDefaultNavigationSection(NavigationSection.AccountsFinance)
            .AddControllersAsServices();

        services.AddHttpsRedirection(options =>
        {
            options.HttpsPort = _configuration["EnvironmentName"].StartsWith("LOCAL") ? 5001 : 443;
        });

        services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { ConnectionString = _configuration["APPINSIGHTS_INSTRUMENTATIONKEY"] });

        if (_configuration["EnvironmentName"] == "LOCAL" || _configuration["EnvironmentName"] == "DEV")
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _configuration.GetValue<string>("EmployerIncentivesWeb:RedisCacheConnectionString");
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
            .AddEmployerIncentivesService()
            .AddDataEncryptionService()
            .AddVerificationService()
            .AddDataProtection(_configuration);

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