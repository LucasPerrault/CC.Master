using Microsoft.Extensions.DependencyInjection;
using Slack.Domain;
using System.Net.Http.Headers;

namespace Slack.Infra;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSlack(this IServiceCollection services, ISlackConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration?.Token);

        services.AddScoped<ISlackClient, SlackClient>();
        services.AddHttpClient<InternalSlackClient>(c =>
        {
            c.BaseAddress = new Uri("https://slack.com");
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.Token);
        });
        return services;
    }
}
