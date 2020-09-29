using Events.Infra.Configuration;
using Lucca.Core.AspNetCore.Abstractions;
using Lucca.Core.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Web
{
	public static class EventsConfigurer
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, string appName, EventsConfiguration configuration)
		{
			var appEventsConfig = 
				new AppEventsConfiguration
				{
					AppName = appName,
					LuccaCoreEventsConfiguration = new RabbitMqConfiguration
					{
						Host = configuration.Host,
						PublisherConfirmation = true
					}
				};
			return services.AddSingleton(appEventsConfig);
		}

		public static IServiceCollection AddLuccaEvents<TPayload, TTenant, TConsumer>(this IServiceCollection services, string queueName)
			where TTenant : Tenant, new()
			where TConsumer : class, IBasicConsumer<TPayload>
		{
			var config = services.BuildServiceProvider().GetRequiredService<AppEventsConfiguration>();
			services.AddLuccaEvents(config.AppName, config.LuccaCoreEventsConfiguration, builder =>
			{
				builder.RegisterBasicConsumer<TPayload, TTenant, TConsumer>(queueName);
			});

			return services;
		}
	}

	public class AppEventsConfiguration
	{
		internal string AppName { get; set; }
		internal RabbitMqConfiguration LuccaCoreEventsConfiguration { get; set; }
	}
}
