using Environments.Application;
using Environments.Domain;
using Environments.Domain.Storage;
using Environments.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Environments.Web
{
    public static class EnvironmentsConfigurer
    {
        public static void ConfigureEnvironments(IServiceCollection services)
        {
            services.AddSingleton<IEnvironmentFilter, EnvironmentFilter>();

            services.AddScoped<IEnvironmentsStore, EnvironmentsStore>();
            services.AddScoped<EnvironmentsRepository>();
        }
    }
}
