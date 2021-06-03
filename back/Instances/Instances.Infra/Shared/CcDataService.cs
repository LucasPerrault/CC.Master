using Instances.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Remote.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Shared
{

    internal class DeleteInstanceRequestDto
    {
        public List<string> Tenants { get; set; }
        public Uri CallbackUri { get; set; }
        public string CallbackAuthorizationHeader { get; set; }
    }


    internal class DuplicateInstanceRequestDtoWithCallback
    {
        public TenantDto SourceTenant { get; set; }
        public string TargetTenant { get; set; }
        public List<UriLinkDto> PostRestoreScripts { get; set; }
        public Uri CallbackUri { get; set; }
        public string CallbackAuthorizationHeader { get; set; }

        public DuplicateInstanceRequestDtoWithCallback(DuplicateInstanceRequestDto dto)
        {
            SourceTenant = dto.SourceTenant;
            TargetTenant = dto.TargetTenant;
            PostRestoreScripts = dto.PostRestoreScripts;
        }
    }

    public class CcDataService : ICcDataService
    {

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
            var requestWithCallback = new DuplicateInstanceRequestDtoWithCallback(duplicateInstanceRequest)
            {
                CallbackUri = GetCallbackUri(callbackPath),
                CallbackAuthorizationHeader = GetCallbackAuthHeader()
            };

            var uri = new Uri(GetCcDataBaseUri(cluster), "/api/v1/duplicate-instance");
            try
            {
                var result = await _httpClient.PostAsync(uri, requestWithCallback.ToJsonPayload());

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

        // Changements à reporter dans le CClithe
        public Uri GetCcDataBaseUri(string cluster)
        {
            var subdomain = GetCcDataSubdomainPart();
            var clusterSubdomainPart = ClusterNameConvertor.GetShortName(cluster);

            return new Uri($"{_ccDataConfiguration.Scheme}://{subdomain}.{clusterSubdomainPart}.{_ccDataConfiguration.Domain}");
        }

        // Changements à reporter dans le CClithe
        private string GetCcDataSubdomainPart()
        {
            return _ccDataConfiguration.ShouldTargetBeta
                ? "cc-data.beta"
                : "cc-data";
        }
    }
}
