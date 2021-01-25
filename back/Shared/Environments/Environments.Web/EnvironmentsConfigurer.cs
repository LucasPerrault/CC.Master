using Environments.Application;
using Environments.Domain;
using Environments.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Environments.Web
{
    public static class EnvironmentsConfigurer
    {
        public static void ConfigureEnvironments(IServiceCollection services)
        {
            services.AddSingleton<IEnvironmentFilter, EnvironmentFilter>();

            services.AddScoped<EnvironmentsStore>();
            services.AddScoped<EnvironmentsRepository>();
        }
    }
}
