using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace Cache.Web
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly RedisCacheService _redisService;

        public RedisHealthCheck(RedisCacheService redisService)
        {
            _redisService = redisService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = _redisService.IsHealthy()
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy();

            return Task.FromResult(result);
        }
    }
}
