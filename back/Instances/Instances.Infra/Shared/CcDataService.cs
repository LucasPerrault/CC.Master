using Instances.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Instances.Infra.Shared
{
    public class CcDataService : ICcDataService
    {
        private static readonly Regex ClusterNumberExtractor = new Regex(@"([a-z]+)(\d+)", RegexOptions.Compiled);

        private readonly HttpClient _httpClient;
        private readonly CcDataConfiguration _ccDataConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CcDataService(HttpClient httpClient, CcDataConfiguration ccDataConfiguration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _ccDataConfiguration = ccDataConfiguration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task StartDuplicateInstanceAsync(DuplicateInstanceRequestDto duplicateInstanceRequest, string cluster, string callbackPath)
        {
            var body = JToken.FromObject(duplicateInstanceRequest);
            var authorizationHeader = $"Cloudcontrol webservice={_ccDataConfiguration.Token}";
            body["CallbackUri"] = new UriBuilder
            {
                Scheme = _httpContextAccessor.HttpContext.Request.Scheme,
                Host = _httpContextAccessor.HttpContext.Request.Host.Host,
                Path = callbackPath
            }.Uri;
            body["CallbackAuthorizationHeader"] = authorizationHeader;
            var content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);

            var uri = new Uri(GetCcDataBaseUri(cluster), "/api/v1/duplicate-instance");
            var result = await _httpClient.PostAsync(uri, content);

            result.EnsureSuccessStatusCode();
        }

        public Uri GetCcDataBaseUri(string cluster)
        {
            cluster = cluster.ToLower();
            var match = ClusterNumberExtractor.Match(cluster);
            int? clusterNumber = null;
            if (match.Success)
            {
                cluster = match.Groups[1].Value;
                clusterNumber = int.Parse(match.Groups[2].Value);
            }
            if (cluster == "green")
            {
                clusterNumber = null;
            }
            cluster = cluster switch
            {
                "cluster" => "c",
                "demo" => "dm",
                "preview" => "pm",
                "formation" => "fm",
                "green" => "ch",
                "security" => "se",
                _ => throw new NotSupportedException($"Cluster name is not supported : {cluster}")
            };
            return new Uri($"http://cc-data.{cluster}{clusterNumber?.ToString() ?? ""}.lucca.local");
        }
    }
}
