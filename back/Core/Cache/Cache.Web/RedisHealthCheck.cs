using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace Cache.Web
{
    public interface IRedisHealthService
    {
        bool IsHealthy();
    }

    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IRedisHealthService _redisService;

        public RedisHealthCheck(IRedisHealthService redisService)
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
