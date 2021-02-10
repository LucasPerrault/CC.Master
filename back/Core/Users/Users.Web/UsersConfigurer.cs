using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Extensions;
using Users.Domain;
using Users.Infra;
using Users.Infra.Storage.Stores;

namespace Users.Web
{
    public class UsersConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, UsersConfiguration config)
        {
            services.AddHttpClient<IUsersService, UsersService>(client =>
            {
                client.WithUserAgent(nameof(UsersService))
                    .WithBaseAddress(config.ServerUri, config.UsersEndpointPath);
            });

            services.AddScoped<UsersStore>();
        }
    }
}
