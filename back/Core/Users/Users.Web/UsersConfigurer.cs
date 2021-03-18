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

            services.AddHttpClient<IUsersSyncService, UsersSyncService>(client =>
            {
                client.WithUserAgent(nameof(UsersSyncService))
                    .WithBaseAddress(config.ServerUri, config.AllUsersEndpointPath)
                    .WithAuthScheme("Lucca")
                    .AuthenticateAsApplication(config.UserFetchToken);

            });

            services.AddScoped<UsersStore>();
        }
    }
}
