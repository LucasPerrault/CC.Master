﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Proxy.Infra.Services
{
    public class LegacyHealthzService : HostRemoteService
    {
        private const string HealthzSubroute = "healthz";

        protected override string RemoteApiDescription => "Legacy Healthz";

        public LegacyHealthzService(HttpClient httpClient)
            : base(httpClient)
        { }

        internal async Task<HealthCheckResult> GetLegacyHealthAsync()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fields", LegacyHealthz.ApiFields }
            };
            try
            {
                var legacyHealthzReponse = await GetGenericObjectResponseAsync<LegacyHealthz>(HealthzSubroute, queryParams);
                return legacyHealthzReponse.GetHealthCheckResult();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }

        protected override string GetErrorMessage(string s)
        {
            throw new NotImplementedException();
        }

        private class LegacyHealthz
        {
            public static readonly string ApiFields = $"{nameof(Status)}";

            public string Status { get; set; }

            public HealthCheckResult GetHealthCheckResult()
            {
                return Status switch
                {
                    "Healthy" => HealthCheckResult.Healthy(),
                    _ => HealthCheckResult.Unhealthy()
                };
            }
        }
    }
}
