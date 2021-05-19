using Authentication.Web;
using Billing.Contracts.Infra.Storage;
using Billing.Web;
using Cache.Web;
using CloudControl.Web.Configuration;
using CloudControl.Web.Exceptions;
using CloudControl.Web.Spa;
using Distributors.Infra.Storage;
using Distributors.Web;
using Email.Web;
using Environments.Infra.Storage;
using Environments.Web;
using Instances.Infra.Storage;
using Instances.Web;
using IpFilter.Infra.Storage;
using IpFilter.Web;
using Lucca.Core.Api.Abstractions;
using Lucca.Core.Api.Queryable.EntityFrameworkCore;
using Lucca.Core.Api.Web;
using Lucca.Core.AspNetCore.Healthz;
using Lucca.Core.AspNetCore.Middlewares;
using Lucca.Core.AspNetCore.Tenancy;
using Lucca.Logs.AspnetCore;
using Lucca.Logs.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Proxy.Web;
using Remote.Infra;
using Rights.Web;
using Salesforce.Web;
using Storage.Infra.Context;
using Storage.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using TeamNotification.Web;
using Tools.Web;
using Users.Infra.Storage;
using Users.Web;

namespace CloudControl.Web
{
    public class ServicesConfiguration
    {
        private IConfiguration _configuration { get; }
        private IWebHostEnvironment _hostingEnvironment { get; }

        public ServicesConfiguration(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _hostingEnvironment = env ?? throw new ArgumentNullException(nameof(env));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = ConfigureConfiguration(services);
            ToolsConfigurer.ConfigureTools(services);
            ConfigureCulture(services);
            ConfigureHttpContext(services);
            ConfigureHealthCheck(services, configuration);
            ConfigureApi(services);
            ConfigureLogs(services);
            // ConfigureSpa(services);
            ConfigureCache(services, configuration);
            ConfigureNotifications(services, configuration);
            ConfigureProxy(services);
            ConfigureIpFilter(services);
            ConfigureTenancy(services);
            ConfigureStorage(services);
            ConfigureSharedDomains(services, configuration);
            ConfigureAuthentication(services, configuration);
            ConfigureRights(services, configuration);
            ConfigureSalesforce(services, configuration);
            ConfigureBilling(services, configuration);
            ConfigureInstances(services, configuration);
            ConfigureEmails(services, configuration);
        }

        private void ConfigureCulture(IServiceCollection services)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("fr");

            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("fr"),
                };
                opts.DefaultRequestCulture = new RequestCulture("fr");
                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });
        }

        public virtual AppConfiguration ConfigureConfiguration(IServiceCollection services)
        {
            services.Configure<AppConfiguration>(_configuration);

            var config = _configuration.Get<AppConfiguration>();
            services.AddSingleton(config.LegacyCloudControl);

            return config;
        }

        public virtual void ConfigureHttpContext(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<JsonSerializer>();
        }

        public virtual void ConfigureHealthCheck(IServiceCollection services, AppConfiguration configuration)
        {
            ProxyConfigurer.ConfigureLegacyHealthzServices(services, configuration.LegacyCloudControl);

            services
                .AddHealthCheck(o =>
                    {
                        o.ServiceGuid = new Guid("101DFDBD-2438-43D1-9D22-63D1C46B3412");// TODO
                        o.ServiceName = AppConfiguration.AppName;
                    }
                )
                .AddLegacyCheck()
                .AddRedisCheck();
        }

        public virtual void ConfigureApi(IServiceCollection services)
        {
            services.AddControllers();

            services.AddLuccaApi(luccaApiBuilder =>
            {
                luccaApiBuilder
                    .SetPagingDefaultLimit(100)
                    .AddModelBinding()
                    .AddEntityFrameworkQuerying()
                    .ConfigureLuccaApiForInstances();
            });
            services.AddMvc().AddLuccaApi(o =>
            {
                o.ShouldIncludeFullExceptionDetails = _hostingEnvironment.IsDevelopment();
            })
            .AddMvcOptions(
                options => options.Filters.Add<HandleDomainExceptionsFilter>()
            );
        }

        public virtual void ConfigureCache(IServiceCollection services, AppConfiguration configuration)
        {
            RedisCacheConfigurer.ConfigureRedis(services, configuration.Redis);
        }

        public virtual void ConfigureProxy(IServiceCollection services)
        {
            ProxyConfigurer.ConfigureServices(services);
        }

        public virtual void ConfigureIpFilter(IServiceCollection services)
        {
            services.Configure<LuccaSecuritySettings>(_configuration.GetSection("LuccaSecurity"));
            IpFilterConfigurer.ConfigureServices(services);
        }

        public virtual void ConfigureTenancy(IServiceCollection services)
        {
            services.AddTenancy(t => { }, DatabaseMode.MultiTenant);
        }

        public virtual void ConfigureNotifications(IServiceCollection services, AppConfiguration configuration)
        {
            TeamNotificationConfigurer.ConfigureTeamNotification(services, configuration.Slack);
        }

        public virtual void ConfigureStorage(IServiceCollection services)
        {
            StorageConfigurer.ConfigureServices(services, _configuration);
            services.ConfigureContext<EnvironmentsDbContext>(_hostingEnvironment);
            services.ConfigureContext<DistributorsDbContext>(_hostingEnvironment);
            services.ConfigureContext<IpFilterDbContext>(_hostingEnvironment);
            services.ConfigureContext<ContractsDbContext>(_hostingEnvironment);
            services.ConfigureContext<UsersDbContext>(_hostingEnvironment);
            services.ConfigureContext<InstancesDbContext>(_hostingEnvironment);
        }

        public virtual void ConfigureSharedDomains(IServiceCollection services, AppConfiguration configuration)
        {
            DistributorsConfigurer.ConfigureServices(services);
            UsersConfigurer.ConfigureServices(services, configuration.Users);
            EnvironmentsConfigurer.ConfigureEnvironments(services);
            RemoteConfigurer.ConfigureRemote(services);
        }

        public virtual void ConfigureAuthentication(IServiceCollection services, AppConfiguration configuration)
        {
            AuthConfigurer.ConfigureServices(services, configuration.Authentication);
        }

        public virtual void ConfigureEmails(IServiceCollection services, AppConfiguration configuration)
        {
            EmailConfigurer.ConfigureServices(services, configuration.Email);
        }

        public virtual void ConfigureRights(IServiceCollection services, AppConfiguration configuration)
        {
            RightsConfigurer.ConfigureServices(services, configuration.Rights);
        }

        public virtual void ConfigureBilling(IServiceCollection services, AppConfiguration configuration)
        {
            BillingConfigurer.ConfigureServices(services, configuration.LegacyCloudControl, configuration.BillingContracts);
        }

        public virtual void ConfigureInstances(IServiceCollection services, AppConfiguration configuration)
        {
            InstancesConfigurer.ConfigureServices(services, configuration.Instances);
        }

        public virtual void ConfigureLogs(IServiceCollection services)
        {
            services
                .AddSingleton<IExceptionQualifier, CloudControlExceptionsQualifier>()
                .AddLogging(l =>
                {
                    l.AddLuccaLogs(_configuration.GetSection(AppConfiguration.LuccaLoggerOptionsKey), AppConfiguration.AppName);
                });
        }

        public virtual void ConfigureSpa(IServiceCollection services)
        {
            services.RegisterFrontApplication(_hostingEnvironment);
        }

        public virtual void ConfigureSalesforce(IServiceCollection services, AppConfiguration configuration)
        {
            SalesforceConfigurer.ConfigureServices(services, configuration.Salesforce);
        }
    }
}
