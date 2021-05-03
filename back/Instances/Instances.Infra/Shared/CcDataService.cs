using Instances.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Remote.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Instances.Infra.Shared
{

    internal class DeleteInstanceRequestDto
    {
        public List<string> Tenants { get; set; }
        public Uri CallbackUri { get; set; }
        public string CallbackAuthorizationHeader { get; set; }
    }

    public class CcDataService : ICcDataService
    {
        private static readonly Regex ClusterNumberExtractor = new Regex(@"([a-z]+)(\d+)", RegexOptions.Compiled);

        private readonly HttpClient _httpClient;
        private readonly CcDataConfiguration _ccDataConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CcDataService> _logger;

        public CcDataService(
            HttpClient httpClient,
            CcDataConfiguration ccDataConfiguration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CcDataService> logger
        )
        {
            _httpClient = httpClient;
            _ccDataConfiguration = ccDataConfiguration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task StartDuplicateInstanceAsync(DuplicateInstanceRequestDto duplicateInstanceRequest, string cluster, string callbackPath)
        {
            var body = JToken.FromObject(duplicateInstanceRequest);
            body["CallbackUri"] = GetCallbackUri(callbackPath);
            body["CallbackAuthorizationHeader"] = GetCallbackAuthHeader();

            var uri = new Uri(GetCcDataBaseUri(cluster), "/api/v1/duplicate-instance");
            try
            {

                var result = await _httpClient.PostAsync(uri, body.ToJsonPayload());

                result.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException re)
            {
                _logger.LogError(re, $"{re.Message}({uri.OriginalString})");
                throw;
            }
        }

        public Task DeleteInstanceAsync(string subdomain, string cluster, string callbackPath)
        {
            return DeleteInstancesAsync(new List<string> { subdomain }, cluster, callbackPath);
        }

        public async Task DeleteInstancesAsync(IEnumerable<string> subdomains, string cluster, string callbackPath)
        {
            var request = new DeleteInstanceRequestDto
            {
                Tenants = subdomains.ToList(),
                CallbackUri = GetCallbackUri(callbackPath),
                CallbackAuthorizationHeader =  GetCallbackAuthHeader()
            };

            var uri = new Uri(GetCcDataBaseUri(cluster), "/api/v1/delete-instance");
            var result = await _httpClient.PostAsync(uri, request.ToJsonPayload());

            result.EnsureSuccessStatusCode();
        }

        private Uri GetCallbackUri(string callbackPath)
        {
            return new UriBuilder
            {
                Scheme = _httpContextAccessor.HttpContext.Request.Scheme,
                Host = _httpContextAccessor.HttpContext.Request.Host.Host,
                Path = callbackPath
            }.Uri;
        }

        private string GetCallbackAuthHeader()
        {
            return $"Cloudcontrol application={_ccDataConfiguration.InboundToken}";
        }

        public Uri GetCcDataBaseUri(string cluster)
        {
            var parsedCluster = GetParsedCluster(cluster);

            var subdomain = GetCcDataSubdomainPart();
            var clusterSubdomainPart = GetClusterSubdomainPart(parsedCluster);

            return new Uri($"{_ccDataConfiguration.Scheme}://{subdomain}.{clusterSubdomainPart}.{_ccDataConfiguration.Domain}");
        }

        private string GetClusterSubdomainPart(ParsedCluster parsedCluster)
        {
            return GetClusterName(parsedCluster) + GetClusterNumber(parsedCluster);
        }

        private string GetCcDataSubdomainPart()
        {
            return _ccDataConfiguration.ShouldTargetBeta
                ? "cc-data.beta"
                : "cc-data";
        }

        private static string GetClusterName(ParsedCluster context)
        {
            return context.Name switch
            {
                "cluster" => "c",
                "demo" => "dm",
                "preview" => "pm",
                "formation" => "fm",
                "green" => "ch",
                "security" => "se",
                "recette" => "re",
                _ => throw new NotSupportedException($"Cluster name is not supported : {context.Name}")
            };
        }

        private string GetClusterNumber(ParsedCluster context)
        {
            return context.Name switch
            {
                "green" => string.Empty,
                "demo" when !context.Number.HasValue => "1",
                _ => context.Number.HasValue ? context.Number.ToString() : string.Empty
            };
        }

        private ParsedCluster GetParsedCluster(string cluster)
        {
            cluster = cluster.ToLower();
            var match = ClusterNumberExtractor.Match(cluster);

            if (match.Success)
            {
                return new ParsedCluster
                {
                    Name = match.Groups[1].Value,
                    Number = int.Parse(match.Groups[2].Value)
                };
            }

            return new ParsedCluster
            {
                Name = cluster
            };
        }

        private class ParsedCluster
        {
            public string Name { get; set; }
            public int? Number { get; set; }
        }
    }
}
