using Cache.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Cache.Web
{
    public class RedisConfiguration
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public int KeyInvalidationInMinutes { get; set; }
    }

    public static class RedisCacheConfigurer
    {
        public static void ConfigureRedis(IServiceCollection service, RedisConfiguration configuration)
        {
            service.AddSingleton<ICacheService, RedisCacheService>(p => CacheService(configuration));
            service.AddSingleton(p => CacheService(configuration));
        }

        private static RedisCacheService CacheService(RedisConfiguration configuration)
        {
            var options = new ConfigurationOptions
            {
                EndPoints = { configuration.Host },
                Password = configuration.Password
            };

            try
            {
                var multiplexer = ConnectionMultiplexer.Connect(options);
                return new RedisCacheService(multiplexer, configuration.KeyInvalidationInMinutes);
            }
            catch (RedisConnectionException)
            {
                return new RedisCacheService(null, configuration.KeyInvalidationInMinutes);
            }
        }



        public static IHealthChecksBuilder AddRedisCheck(this IHealthChecksBuilder builder)
        {
            builder.AddCheck<RedisHealthCheck>("redis_check");
            return builder;
        }
    }
}
