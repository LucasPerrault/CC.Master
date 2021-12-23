using IpFilter.Domain;
using IpFilter.Domain.Accessors;
using IpFilter.Infra.Storage.Stores;
using Lucca.Core.AspNetCore.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Resources.Translations;
using System;

namespace IpFilter.Web
{
    public static class IpFilterConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IIpFilterTranslations, IpFilterTranslations>();
            services.AddScoped<IpFilterRequestCreationService>();
            services.AddScoped<IIpFilterAuthorizationRequestStore, IpFilterAuthorizationRequestStore>();
            services.AddScoped<IIpFilterEmails, IpFilterEmails>();
            services.AddScoped<IpFilterService>();
            services.AddScoped<IIpFilterAuthorizationStore, IpFilterAuthorizationStore>();
            services.AddScoped<IIpAccessor, CurrentUserIpAccessor>();
            services.AddScoped<IUserAgentAccessor, UserAgentAccessor>();
            services.AddLuccaIpWhitelist<CurrentUserIpAccessor, IpRejectionService>();
        }
    }
}
