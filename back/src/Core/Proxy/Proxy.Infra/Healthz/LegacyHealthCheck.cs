using Core.Proxy.Infra.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Proxy.Infra.Healthz
{
    public class LegacyHealthCheck : IHealthCheck
    {
        private readonly LegacyHealthzService _healthzService;

        public LegacyHealthCheck(LegacyHealthzService healthzService)
        {
            _healthzService = healthzService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return _healthzService.GetLegacyHealthAsync();
        }
    }
}
