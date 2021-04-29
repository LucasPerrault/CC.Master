using Microsoft.Extensions.DependencyInjection;
using TeamNotification.Abstractions;

namespace TeamNotification.Web
{
    public static class TeamNotificationConfigurer
    {
        public static void ConfigureTeamNotification(IServiceCollection service, SlackConfiguration configuration)
        {
            service.AddSingleton(configuration.Hooks);
            service.AddHttpClient<ITeamNotifier, TeamNotifier>();
        }
    }
}
