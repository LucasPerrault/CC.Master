using Environments.Application;
using Environments.Domain;
using Environments.Domain.Storage;
using Environments.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;
using System;

namespace Environments.Web
{
    public class EnvironmentConfiguration
    {
        public Uri LegacyHost { get; set; }
        public Guid LegacyToken { get; set; }
        public EnvironmentRenamingConfiguration Renaming { get; set; }
    }

    public static class EnvironmentsConfigurer
    {
        public static void ConfigureEnvironments(IServiceCollection services, EnvironmentConfiguration configuration)
        {
            services.AddHttpClient<IEnvironmentsRemoteStore, EnvironmentsRemoteStore>(client =>
            {
                client
                    .WithUserAgent(nameof(EnvironmentsStore))
                    .WithBaseAddress(configuration.LegacyHost, "/api/v3/environments")
                    .WithAuthScheme("CloudControl").AuthenticateAsApplication(configuration.LegacyToken);
            });
            services.AddScoped<IEnvironmentsStore, EnvironmentsStore>();
            services.AddScoped<EnvironmentsRepository>();
            services.AddScoped<EnvironmentRightsFilter>();
            services.AddScoped<IEnvironmentLogsStore, EnvironmentLogsStore>();

            services.AddSingleton(configuration.Renaming);
            services.AddScoped<IEnvironmentRenamingService, EnvironmentRenamingService>();
            services.AddScoped<IEnvironmentsRenamingStore, EnvironmentsRenamingStore>();
        }
    }
}
